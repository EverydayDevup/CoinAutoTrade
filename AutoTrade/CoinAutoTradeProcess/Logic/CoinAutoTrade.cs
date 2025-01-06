using SharedClass;

namespace CoinAutoTradeProcess;

public class CoinAutoTrade(IMarket? market, CoinAutoTradeProcessClient client)
{
    private IMarket? Market { get; } = market;
    private const int Delay = 200;
    private bool IsRunning { get; set; }

    private List<CoinTradeData>? _coinTradeDataList;
    private CoinAutoTradeProcessClient Client { get; } = client;
    private string CoinAutoTradeLogDirectoryPath => Path.Combine(Directory.GetCurrentDirectory(), nameof(CoinAutoTrade), Client.MarketType.ToString());

    public void Reload(List<CoinTradeData> coinTradeDataList)
    {
        _coinTradeDataList = coinTradeDataList;
        
        if (!IsRunning)
            Run();
    }

    private async void Run()
    {
        IsRunning = true;
        while (IsRunning)
        {
            if (_coinTradeDataList == null)
                return;

            var coinTradeDataList = new List<CoinTradeData>();
            coinTradeDataList.AddRange(_coinTradeDataList);
            
            foreach (var coinTradeData in coinTradeDataList)
                await TradeAsync(coinTradeData);
            
            await Task.Delay(Delay);
        }
    }

    private async Task TradeAsync(CoinTradeData coinTradeData)
    {
        switch (coinTradeData.State)
        {
            case ECoinTradeState.Ready:
                await ReadyTradeAsync(coinTradeData);
                break;
            case ECoinTradeState.Progress:
                await ProgressTradeAsync(coinTradeData);
                break;
            case ECoinTradeState.Completed:
            case ECoinTradeState.Stop:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task ReadyTradeAsync(CoinTradeData coinTradeData)
    {
        if (Market == null)
            return;

        var isRefresh = false;
        
        switch (coinTradeData.TradeType)
        {
            case ECoinTradeType.AutoTrade:
                coinTradeData.State = ECoinTradeState.Progress;
                isRefresh = true;
                break;
            case ECoinTradeType.NewCoin:
                var marketCodeResponse = await Market.RequestMarketCodes();
                if (marketCodeResponse != null)
                {
                    if (marketCodeResponse.IsExist(coinTradeData.MarketCode))
                    {
                        coinTradeData.State = ECoinTradeState.Progress;
                        isRefresh = true;
                    }
                }
                break;
            case ECoinTradeType.Pumping:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (isRefresh)
        {
           await Client.RequestInnerAddOrUpdateCoinTradeDataAsync("ready to progress", coinTradeData);
        }

        await ProgressTradeAsync(coinTradeData);
    }

    private async Task ProgressTradeAsync(CoinTradeData coinTradeData)
    {
        if (Market == null)
            return;
        
        // 첫 구매 시
        if (coinTradeData.BuyCount == 0)
        {
            var isRefresh = false;
            
            // 초기 구매 가격이 설정된 경우 해당 가격으로 값을 설정
            if (coinTradeData.InitBuyPrice > 0)
            {
                isRefresh = true;
                coinTradeData.BuyPrice = coinTradeData.InitBuyPrice;
                coinTradeData.InitBuyPrice = -1;
            }

            // 초기 판매 가격이 설정된 경우 해당 가격으로 판매 가격을 설정함
            if (coinTradeData.MaxSellPrice > 0)
            {
                isRefresh = true;
                coinTradeData.SellPrice = coinTradeData.MaxSellPrice;
                coinTradeData.MaxSellPrice = -1;
            }

            if (isRefresh)
            {
                await Client.RequestInnerAddOrUpdateCoinTradeDataAsync("init price setting", coinTradeData);
            }
        }
        
        var marketTickerResponse = await Market.RequestTicker(coinTradeData.MarketCode);
        var marketTickerJson = marketTickerResponse?.GetMarketTickerJson(coinTradeData.MarketCode);
        if (marketTickerJson == null)
            return;

        var ticker = marketTickerJson.Price;
        var message =  $"{coinTradeData.MarketCode} {nameof(ticker)} = {ticker}" +
            $" {nameof(coinTradeData.BuyPrice)} = {coinTradeData.BuyPrice}" +
            $" {nameof(coinTradeData.SellPrice)} = {coinTradeData.SellPrice}";
        
        Client.LoggerService.ConsoleLog(message);
        Client.LoggerService.FileLog(CoinAutoTradeLogDirectoryPath, message);
        
        if (ticker >= coinTradeData.BuyPrice)
            await BuyTradeAsync(coinTradeData);
        else if (ticker <= coinTradeData.SellPrice)
            await SellTradeAsync(coinTradeData);
    }

    private async Task BuyTradeAsync(CoinTradeData coinTradeData)
    {
        if (Market == null)
            return;

        MarketBalanceJson? marketBalanceJson = null;
        var investAmount = coinTradeData.InvestRoundAmount;
        double buyPrice;

        var loggerService = Client.LoggerService;
        var message = $"{coinTradeData.MarketCode} {nameof(BuyTradeAsync)}";
        loggerService.FileLog(CoinAutoTradeLogDirectoryPath, message);
        loggerService.ConsoleLog(message);
        
        do
        {
            var balanceResponse = await Market.RequestBalance("KRW");
            marketBalanceJson = balanceResponse?.GetMarketBalance("KRW");
            if (marketBalanceJson == null)
            {
                message = $"{marketBalanceJson} is null";
                loggerService.ConsoleLog(message);
                loggerService.FileLog(CoinAutoTradeLogDirectoryPath, message);
                return;
            }

            var orderBookResponse = await Market.RequestMarketOrderBook(coinTradeData.MarketCode);
            var orderBooks = orderBookResponse?.GetAskOrderBooks(coinTradeData.MarketCode);
            if (orderBooks is not { Count: > 0 })
            {
                message = $"{orderBooks} is null";
                loggerService.ConsoleLog(message);
                loggerService.FileLog(CoinAutoTradeLogDirectoryPath, message);
                return;
            }

            var (price, size) = orderBooks[0];
            if (size <= 0)
            {
                if (orderBooks.Count < 2)
                {
                    message = $"{orderBooks} {nameof(size)} is 0";
                    loggerService.ConsoleLog(message);
                    loggerService.FileLog(CoinAutoTradeLogDirectoryPath, message);
                    return;
                }
                
                (price, _) = orderBooks[1];
            }
            
            buyPrice = price;
            
            var volume = investAmount / price;
            var buyResponse = await Market.RequestBuy(coinTradeData.MarketCode, volume, price);
            var uuid = buyResponse?.Result?.Uuid;
            if (string.IsNullOrEmpty(uuid))
            {
                message = $"{nameof(buyResponse)} {uuid} is null";
                loggerService.ConsoleLog(message);
                loggerService.FileLog(CoinAutoTradeLogDirectoryPath, message);
                return;
            }

            await Task.Delay(Delay);
            
            var orderResponse = await Market.RequestOrder(uuid);
            var orderJson = orderResponse?.Result;
            if (orderJson == null)
            {
                message = $"{orderJson} is null";
                loggerService.ConsoleLog(message);
                loggerService.FileLog(CoinAutoTradeLogDirectoryPath, message);
                return;
            }

            investAmount -= orderJson.GetExecutedVolume() * price;

            if (orderJson.GetState() != EMarketOrderState.Done)
            {
                await Market.RequestCancelOrder(uuid);
                await Task.Delay(Delay);
            }

            message = $"{nameof(BuyTradeAsync)} [{Client.MarketType}] {coinTradeData.MarketCode} price = {price} amount = {orderJson.GetExecutedVolume()}";
            await loggerService.TelegramLogAsync(message);
            loggerService.ConsoleLog(message);
            loggerService.FileLog(CoinAutoTradeLogDirectoryPath, $"{message}");
            
        } while (investAmount >= CoinTradeData.MinInvestAmount && investAmount < marketBalanceJson.Balance);

        coinTradeData.BuyPrice = buyPrice * (1 + coinTradeData.RoundBuyRate);
        coinTradeData.SellPrice = buyPrice * (1 - coinTradeData.RoundSellRate);
        coinTradeData.BuyCount++;
        
        await Client.RequestInnerAddOrUpdateCoinTradeDataAsync("Buy", coinTradeData);
    }

    private async Task SellTradeAsync(CoinTradeData coinTradeData)
    {
        
    }
}

//
// public class CoinTradeProcess
// {
//     public bool IsTrade { get; private set; }
//     public double BuyPrice { get; set; } // 추가 매수 시의 가격
//     public double SellPrice { get; set; } // 손실율 기준으로 매도 시의 가격
//     
//     private readonly IMarket market;
//     private readonly CoinConfig coinConfig;
//     
//     private int buyCount; // 현재까지 투자 횟수
//
//     public CoinTradeProcess(IMarket market, CoinConfig coinConfig)
//     {
//         IsTrade = true;
//         BuyPrice = 0;
//         SellPrice = 0;
//         
//         this.market = market;
//         this.coinConfig = coinConfig;
//     }
//     
//     public async Task TradeAsync()
//     {
//         if (!IsTrade)
//             return;
//         
//         Console.WriteLine("1. Coin Trade Price Check ");
//         Console.WriteLine(MessageManager.GetLine());
//         var tradePrice = await market.RequestTicker(coinConfig.MarketCode);
//         Console.WriteLine($"{nameof(TradeAsync)} {coinConfig.MarketCode} - {nameof(tradePrice)} : {tradePrice} {nameof(BuyPrice)} : {BuyPrice} {nameof(SellPrice)} : {SellPrice}");
//
//         if (tradePrice <= 0)
//             return;
//
//         // 첫 구매
//         if (buyCount == 0)
//         {
//             // 현재 매도 창을 확인해서, 투자금 대비 구매 가능한 영역까지 구매를 시도함
//             await RequestBuy();
//         }
//         else
//         {
//             // 현재 호가가 구매하려는 목표치만큼 온 경우, 추가매수
//             if (tradePrice >= BuyPrice)
//                 await RequestBuy();
//         }
//         
//         // 매도 여부 확인
//         if (buyCount == 0)
//             return;
//         
//         // 현재 호가가 손실치만큼 왔다면 모두 매도함
//         if (SellPrice > 0 && tradePrice < SellPrice)
//         {
//             await RequestSell();
//             IsTrade = false;
//         }
//         
//         Console.WriteLine(MessageManager.GetLine());
//     }
//
//     private async Task RequestBuy()
//     {
//         var buyAmount = coinConfig.Amount;
//         while (buyAmount > 0)
//         {
//             // 현재 보유한 원화 호가인 후, 구매할 수 있는 금액이 없다면 멈춤
//             var krwAmount = await market.RequestBalance("KRW");
//             if (buyAmount > krwAmount)
//             {
//                 MessageManager.Notify($"{nameof(RequestBuy)} Fail {nameof(krwAmount)} : {krwAmount} {nameof(buyAmount)} : {buyAmount}");
//                 return;
//             }
//             
//             var orderBooks = await market.RequestMarketOrderbook(coinConfig.MarketCode);
//             if (orderBooks?.BuyOrders == null)
//             {
//                 Console.WriteLine($"{nameof(RequestBuy)} Error {nameof(orderBooks.BuyOrders)} is null");
//                 return;
//             }
//
//             if (orderBooks.BuyOrders.Length < 1)
//             {
//                 Console.WriteLine($"{nameof(RequestBuy)} Error  {nameof(orderBooks.BuyOrders)} length less than 1");
//                 return;
//             }
//             
//             var buyOrderbook = orderBooks.BuyOrders[0];
//             double price = buyOrderbook.Price;
//             if (price <= 0)
//             {
//                 Console.WriteLine($"{nameof(RequestBuy)} Error {nameof(price)} is zero");
//                 return;
//             }
//
//             // 급락할 경우 매도를 위해서 더 이상 매수하지 않음
//             if (price < SellPrice)
//             {
//                 Console.WriteLine($"{nameof(RequestBuy)} Buy Stop {nameof(price)} : {price} {nameof(SellPrice)} : {SellPrice}");
//                 return;
//             }
//             
//             var amount = buyAmount / buyOrderbook.Price;
//             var orderAmount = Math.Min(amount, buyOrderbook.Amount);
//             var prevCoinAmount = await market.RequestBalance(coinConfig.Symbol);
//             Console.WriteLine($"{nameof(RequestBuy)} Buy Wait {nameof(price)} : {price} {nameof(orderAmount)} : {orderAmount}");
//             var uuid = await market.RequestBuy(coinConfig.MarketCode, orderAmount, price);
//
//             if (string.IsNullOrEmpty(uuid))
//             {
//                 Console.WriteLine($"{nameof(RequestBuy)} Error {nameof(uuid)} is null");
//                 return;
//             }
//             
//             await Task.Delay(100);
//
//             var isOrder = await market.RequestCheckOrder(uuid);
//             if (isOrder)
//             {
//                 await market.RequestCancelOrder(uuid);
//                 continue;
//             }
//
//             var coinAmount = await market.RequestBalance(coinConfig.Symbol);
//             buyAmount -= ((coinAmount - prevCoinAmount) * price);
//
//             BuyPrice = price * (1 + coinConfig.BuyRate / 100f);
//
//             if (buyCount == 0)
//             {
//                 var calSellPrice = (coinConfig.TotalAmount * 0.01f) / coinAmount;
//                 SellPrice = price - calSellPrice;
//             }
//             else
//             {
//                 SellPrice = price * (1 - coinConfig.SellRate / 100f);
//             }
//
//             buyCount++;
//             MessageManager.Notify($"{nameof(RequestBuy)} Buy Complete {nameof(price)} : {price} {nameof(orderAmount)} : {orderAmount} {nameof(buyAmount)} : {buyAmount}" +
//                                  $" {nameof(BuyPrice)} : {BuyPrice} {nameof(SellPrice)} : {SellPrice} {nameof(buyCount)} : {buyCount}");
//             
//             await Task.Delay(50);
//         }
//     }
//
//     private async Task RequestSell()
//     {
//         // 현재 보유한 코인 개수
//         var sellAmount = await market.RequestBalance(coinConfig.Symbol);
//         while (sellAmount > 0)
//         {
//             var orderBooks = await market.RequestMarketOrderbook(coinConfig.MarketCode);
//             if (orderBooks?.SellOrders == null)
//             {
//                 Console.WriteLine($"{nameof(RequestSell)} Error {nameof(orderBooks.SellOrders)} is null");
//                 return;
//             }
//
//             if (orderBooks.SellOrders.Length < 1)
//             {
//                 Console.WriteLine($"{nameof(RequestBuy)} Error  {nameof(orderBooks.SellOrders)} length less than 1");
//                 return;
//             }
//             
//             var sellOrderbook = orderBooks.SellOrders[0];
//             var price = sellOrderbook.Price;
//             if (price <= 0)
//             {
//                 Console.WriteLine($"{nameof(RequestSell)} Error {nameof(price)} is zero");
//                 return;
//             }
//
//             var orderAmount = Math.Min(sellOrderbook.Amount, sellAmount);
//             Console.WriteLine($"{nameof(RequestSell)} Sell Wait {nameof(price)} : {price} {nameof(orderAmount)} : {orderAmount}");
//             var uuid = await market.RequestSell(coinConfig.MarketCode, orderAmount, price);
//             
//             if (string.IsNullOrEmpty(uuid))
//             {
//                 Console.WriteLine($"{nameof(RequestSell)} Error {nameof(uuid)} is null");
//                 return;
//             }
//             
//             await Task.Delay(100);
//
//             var prevAmount = sellAmount;
//             // 현재 보유한 코인의 개수 
//             sellAmount = await market.RequestBalance(coinConfig.Symbol);
//             var sellCompleteCount = prevAmount - sellAmount;
//             
//             if (sellAmount > 0)
//                 MessageManager.Notify($"{nameof(RequestSell)} Sell Complete {nameof(price)} : {price} {nameof(sellCompleteCount)} : {sellCompleteCount} {nameof(sellAmount)} : {sellAmount}");
//             
//             await Task.Delay(50);
//         }
//     }
// }