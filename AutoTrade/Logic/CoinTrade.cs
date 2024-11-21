using AutoTrade.Config;
using AutoTrade.Market;

namespace AutoTrade.Logic;

public static class CoinTrade
{
    private static Dictionary<string, CoinTradeProcess> dicCoinTradeProcess = new();
    private const int Delay = 1000 / 50;
    
    public static async Task Trade(IMarket market,List<CoinConfig> coinConfigList)
    {
        foreach (var coinConfig in coinConfigList)
        {
            if (!dicCoinTradeProcess.ContainsKey(coinConfig.MarketCode))
                dicCoinTradeProcess.Add(coinConfig.MarketCode, new CoinTradeProcess(market, coinConfig));
        }

        var isTrade = false;
        do
        {
            isTrade = false;
            
            foreach (var (_, process) in dicCoinTradeProcess)
            {
                await process.TradeAsync();
                // 하나라도 true이면 trade는 계속됨
                isTrade |= process.IsTrade;
                await Task.Delay(Delay);
            }
            
        } while (isTrade);
    }
}

public class CoinTradeProcess
{
    public bool IsTrade { get; private set; }
    public double BuyPrice { get; set; } // 추가 매수 시의 가격
    public double SellPrice { get; set; } // 손실율 기준으로 매도 시의 가격
    
    private readonly IMarket market;
    private readonly CoinConfig coinConfig;
    private readonly double buyMaxCount; // 총 투자 횟수
    
    private int buyCount; // 현재까지 투자 횟수

    public CoinTradeProcess(IMarket market, CoinConfig coinConfig)
    {
        IsTrade = true;
        BuyPrice = 0;
        SellPrice = 0;
        
        this.market = market;
        this.coinConfig = coinConfig;

        buyMaxCount = coinConfig.TotalAmount / coinConfig.Amount;
    }
    
    public async Task TradeAsync()
    {
        if (!IsTrade)
            return;
        
        Console.WriteLine("1. Coin Trade Price Check ");
        Console.WriteLine(NotifyManager.GetLine());
        var tradePrice = await market.RequestTicker(coinConfig.MarketCode);
        Console.WriteLine($"{nameof(TradeAsync)} {coinConfig.MarketCode} - {nameof(tradePrice)} : {tradePrice} {nameof(BuyPrice)} : {BuyPrice} {nameof(SellPrice)} : {SellPrice}");

        if (tradePrice <= 0)
            return;

        // 매수 여부 확인
        if (buyCount <= buyMaxCount)
        {
            Console.WriteLine($"2. RequestBuy {nameof(buyCount)} : {buyCount}");
            // 첫 구매
            if (buyCount == 0)
            {
                // 현재 매도 창을 확인해서, 투자금 대비 구매 가능한 영역까지 구매를 시도함
                await RequestBuy();
            }
            else
            {
                // 현재 호가가 구매하려는 목표치만큼 온 경우, 추가매수
                if (tradePrice >= BuyPrice)
                    await RequestBuy();
            }
        }
        
        // 매도 여부 확인
        if (buyCount == 0)
            return;
        
        // 현재 호가가 손실치만큼 왔다면 모두 매도함
        if (SellPrice > 0 && tradePrice < SellPrice)
        {
            Console.WriteLine($"3. RequestSell");
            await RequestSell();
            IsTrade = false;
        }
        
        Console.WriteLine(NotifyManager.GetLine());
    }

    private async Task RequestBuy()
    {
        var buyAmount = coinConfig.Amount;
        while (buyAmount > 0)
        {
            // 현재 보유한 원화 호가인 후, 구매할 수 있는 금액이 없다면 멈춤
            var krwAmount = await market.RequestBalance("KRW");
            if (buyAmount > krwAmount)
            {
                NotifyManager.Notify($"{nameof(RequestBuy)} Fail {nameof(krwAmount)} : {krwAmount} {nameof(buyAmount)} : {buyAmount}");
                return;
            }
            
            var orderBooks = await market.RequestMarketOrderbook(coinConfig.MarketCode);
            if (orderBooks?.BuyOrders == null)
            {
                Console.WriteLine($"{nameof(RequestBuy)} Error {nameof(orderBooks.BuyOrders)} is null");
                return;
            }

            if (orderBooks.BuyOrders.Length < 1)
            {
                Console.WriteLine($"{nameof(RequestBuy)} Error  {nameof(orderBooks.BuyOrders)} length less than 1");
                return;
            }
            
            var buyOrderbook = orderBooks.BuyOrders[0];
            double price = buyOrderbook.Price;
            if (price <= 0)
            {
                Console.WriteLine($"{nameof(RequestBuy)} Error {nameof(price)} is zero");
                return;
            }

            // 급락할 경우 매도를 위해서 더 이상 매수하지 않음
            if (price < SellPrice)
            {
                Console.WriteLine($"{nameof(RequestBuy)} Buy Stop {nameof(price)} : {price} {nameof(SellPrice)} : {SellPrice}");
                return;
            }
            
            var amount = buyAmount / buyOrderbook.Price;
            var orderAmount = Math.Min(amount, buyOrderbook.Amount);
            var prevCoinAmount = await market.RequestBalance(coinConfig.Symbol);
            Console.WriteLine($"{nameof(RequestBuy)} Buy Wait {nameof(price)} : {price} {nameof(orderAmount)} : {orderAmount}");
            var uuid = await market.RequestBuy(coinConfig.MarketCode, orderAmount, price);

            if (string.IsNullOrEmpty(uuid))
            {
                Console.WriteLine($"{nameof(RequestBuy)} Error {nameof(uuid)} is null");
                return;
            }
            
            await Task.Delay(100);

            var isOrder = await market.RequestCheckOrder(uuid);
            if (isOrder)
            {
                await market.RequestCancelOrder(uuid);
                continue;
            }

            var coinAmount = await market.RequestBalance(coinConfig.Symbol);
            buyAmount -= ((coinAmount - prevCoinAmount) * price);

            BuyPrice = price * (1 + coinConfig.BuyRate / 100f);

            if (buyCount == 0)
            {
                var calSellPrice = (coinConfig.TotalAmount * 0.01f) / coinAmount;
                SellPrice = price - calSellPrice;
            }
            else
            {
                SellPrice = price * (1 - coinConfig.SellRate / 100f);
            }

            buyCount++;
            NotifyManager.Notify($"{nameof(RequestBuy)} Buy Complete {nameof(price)} : {price} {nameof(orderAmount)} : {orderAmount} {nameof(buyAmount)} : {buyAmount}" +
                                 $" {nameof(BuyPrice)} : {BuyPrice} {nameof(SellPrice)} : {SellPrice} {nameof(buyCount)} : {buyCount}");
            
            await Task.Delay(50);
        }
    }

    private async Task RequestSell()
    {
        // 현재 보유한 코인 개수
        var sellAmount = await market.RequestBalance(coinConfig.Symbol);
        while (sellAmount > 0)
        {
            var orderBooks = await market.RequestMarketOrderbook(coinConfig.MarketCode);
            if (orderBooks?.SellOrders == null)
            {
                Console.WriteLine($"{nameof(RequestSell)} Error {nameof(orderBooks.SellOrders)} is null");
                return;
            }

            if (orderBooks.SellOrders.Length < 1)
            {
                Console.WriteLine($"{nameof(RequestBuy)} Error  {nameof(orderBooks.SellOrders)} length less than 1");
                return;
            }
            
            var sellOrderbook = orderBooks.SellOrders[0];
            var price = sellOrderbook.Price;
            if (price <= 0)
            {
                Console.WriteLine($"{nameof(RequestSell)} Error {nameof(price)} is zero");
                return;
            }

            var orderAmount = Math.Min(sellOrderbook.Amount, sellAmount);
            Console.WriteLine($"{nameof(RequestSell)} Sell Wait {nameof(price)} : {price} {nameof(orderAmount)} : {orderAmount}");
            var uuid = await market.RequestSell(coinConfig.MarketCode, orderAmount, price);
            
            if (string.IsNullOrEmpty(uuid))
            {
                Console.WriteLine($"{nameof(RequestSell)} Error {nameof(uuid)} is null");
                return;
            }
            
            await Task.Delay(100);

            var prevAmount = sellAmount;
            // 현재 보유한 코인의 개수 
            sellAmount = await market.RequestBalance(coinConfig.Symbol);
            var sellCompleteCount = prevAmount - sellAmount;
            
            if (sellAmount > 0)
                NotifyManager.Notify($"{nameof(RequestSell)} Sell Complete {nameof(price)} : {price} {nameof(sellCompleteCount)} : {sellCompleteCount} {nameof(sellAmount)} : {sellAmount}");
            
            await Task.Delay(50);
        }
    }
}