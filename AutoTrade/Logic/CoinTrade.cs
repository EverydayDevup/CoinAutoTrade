using AutoTrade.Config;
using AutoTrade.Market;

namespace AutoTrade.Logic;

public static class CoinTrade
{
    private static Dictionary<string, CoinTradeProcess> dicCoinTradeProcess = new();
    private const int Delay = 1000 / 100;
    
    public static async Task Trade(IMarket market,List<CoinConfig> coinConfigList)
    {
        foreach (var coinConfig in coinConfigList)
        {
            if (!dicCoinTradeProcess.TryGetValue(coinConfig.MarketCode, out var process))
                dicCoinTradeProcess.Add(coinConfig.MarketCode, process = new(market, coinConfig));
        }

        var isTrade = false;
        do
        {
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
    public bool IsTrade { get; set; }
    public double BuyPrice { get; set; } // 추가 매수 시의 가격
    public double SellPrice { get; set; } // 손실율 기준으로 매도 시의 가격
    
    private readonly IMarket market;
    private readonly CoinConfig coinConfig;
    private readonly double buyMaxCount; // 총 투자 횟수
    private readonly double limitAmount; // 전체 자산의 1%를 손실 보는 값
    
    private int buyCount; // 현재까지 투자 횟수

    public CoinTradeProcess(IMarket market, CoinConfig coinConfig)
    {
        IsTrade = true;
        BuyPrice = 0;
        SellPrice = 0;
        
        this.market = market;
        this.coinConfig = coinConfig;

        buyMaxCount = coinConfig.TotalAmount / coinConfig.Amount;
        limitAmount = coinConfig.TotalAmount * 0.01f;
    }
    
    public async Task TradeAsync()
    {
        if (!IsTrade)
            return;
        
        var tradePrice = await market.RequestTicker(coinConfig.MarketCode);
        Console.WriteLine("============================================");
        Console.WriteLine($"{nameof(TradeAsync)} {coinConfig.MarketCode} - {nameof(tradePrice)} : {tradePrice} {nameof(BuyPrice)} : {BuyPrice} {nameof(SellPrice)} : {SellPrice}");

        // 매수 여부 확인
        if (buyCount <= buyMaxCount)
        {
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
        if (tradePrice < SellPrice)
        {
            await RequestSell();
            IsTrade = false;
        }
        
        Console.WriteLine("============================================");
    }

    private async Task RequestBuy()
    {
        var buyAmount = coinConfig.Amount;
        while (buyAmount > 0)
        {
            // 현재 보유한 원화 호가인 후, 구매할 수 있는 금액이 없다면 멈춤
            var currentAmount = await market.RequestBalance("KRW");
            if (buyAmount > currentAmount)
            {
                NotifyManager.Notify($"{nameof(RequestBuy)} Fail {nameof(currentAmount)} : {currentAmount} {nameof(buyAmount)} : {buyAmount}");
                return;
            }
            
            var orderBooks = await market.RequestOrderbook(coinConfig.MarketCode);
            if (orderBooks?.BuyOrderbooks == null)
            {
                Console.WriteLine($"{nameof(RequestBuy)} Error {nameof(orderBooks.BuyOrderbooks)} is null");
                return;
            }

            if (orderBooks.BuyOrderbooks.Length < 1)
            {
                Console.WriteLine($"{nameof(RequestBuy)} Error  {nameof(orderBooks.BuyOrderbooks)} length less than 1");
                return;
            }
            
            var buyOrderbook = orderBooks.BuyOrderbooks[0];
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
                
            // todo 주문
            Console.WriteLine($"{nameof(RequestBuy)} Buy Wait {nameof(price)} : {price} {nameof(orderAmount)} : {orderAmount}");


            var isSuccess = true;
            // 구매 완료 여부 확인 후 구매가 완료 되지 않았다면 주문을 취소함
            // todo 주문 완료 여부 확인

            if (isSuccess)
            {
                // todo 현재 보유한 나의 코인

                var coinAmount = orderAmount;
                
                buyAmount -= (orderAmount * buyOrderbook.Price);
                BuyPrice = price * (1 + coinConfig.BuyRate / 100f);

                if (buyCount == 0)
                {
                    var calSellPrice = limitAmount / coinAmount;
                    SellPrice = price - calSellPrice;
                }
                else
                {
                    SellPrice = price * (1 + coinConfig.SellRate / 100f);
                }

                buyCount++;
                NotifyManager.Notify($"{nameof(RequestBuy)} Buy Complete {nameof(price)} : {price} {nameof(orderAmount)} : {orderAmount} {nameof(buyAmount)} : {buyAmount}" +
                                     $" {nameof(BuyPrice)} : {BuyPrice} {nameof(SellPrice)} : {SellPrice} {nameof(buyCount)} : {buyCount}");
            }
        }
    }

    private async Task RequestSell()
    {
        // 현재 보유한 코인 개수
        var sellAmount = 0;
        while (sellAmount > 0)
        {
            var orderBooks = await market.RequestOrderbook(coinConfig.MarketCode);
            if (orderBooks?.SellOrderbooks == null)
            {
                Console.WriteLine($"{nameof(RequestSell)} Error {nameof(orderBooks.SellOrderbooks)} is null");
                return;
            }

            if (orderBooks.SellOrderbooks.Length < 1)
            {
                Console.WriteLine($"{nameof(RequestBuy)} Error  {nameof(orderBooks.SellOrderbooks)} length less than 1");
                return;
            }
            
            var sellOrderbook = orderBooks.SellOrderbooks[0];
            double price = sellOrderbook.Price;
            if (price <= 0)
            {
                Console.WriteLine($"{nameof(RequestSell)} Error {nameof(price)} is zero");
                return;
            }

            var orderAmount = Math.Min(sellOrderbook.Amount, sellAmount);
            // todo 주문
            Console.WriteLine($"{nameof(RequestSell)} Sell Wait {nameof(price)} : {price} {nameof(orderAmount)} : {orderAmount}");
            
            // 구매 완료 여부 확인 후 구매가 완료 되지 않았다면 주문을 취소함
            // todo 주문 완료 여부 확인

            // 현재 보유한 코인의 개수 
            var currentAmount = 0;
            var sellCompleteCount = sellAmount - currentAmount;
            sellAmount = currentAmount;
            
            if (sellCompleteCount > 0)
                NotifyManager.Notify($"{nameof(RequestSell)} Sell Complete {nameof(price)} : {price} {nameof(sellCompleteCount)} : {sellCompleteCount} {nameof(sellAmount)} : {sellAmount}");
        }
    }
}