using SharedClass;

namespace CoinAutoTradeProcess;

public class CoinAutoTrade(IMarket? market, CoinAutoTradeProcessClient client)
{
    private IMarket? Market { get; } = market;
    private const int Delay = 50;
    private bool IsRunning { get; set; }

    private List<CoinTradeData>? _coinTradeDataList;
    private CoinAutoTradeProcessClient Client { get; } = client;
    private string CoinAutoTradeLogDirectoryPath => Path.Combine(nameof(CoinAutoTrade), Client.MarketType.ToString());

    public void Reload(List<CoinTradeData>? coinTradeDataList)
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
            Client.LoggerService.ConsoleLog("Waiting for data to load...");
            
            if (_coinTradeDataList == null || _coinTradeDataList.Count == 0)
            {
                await Task.Delay(Delay);
                continue;
            }

            var coinTradeDataList = new List<CoinTradeData>();
            coinTradeDataList.AddRange(_coinTradeDataList);

            foreach (var coinTradeData in coinTradeDataList)
            {
                await TradeAsync(coinTradeData);
                await Task.Delay(Delay);
            }
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

        MarketBalanceJson? marketBalanceJson;
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

            if (orderJson.GetState() != EMarketOrderState.Done)
            {
                var cancelOrderResponse = await Market.RequestCancelOrder(uuid);
                var cancelJson = cancelOrderResponse?.Result;
                if (cancelJson == null)
                {
                    message = $"{cancelJson} is null";
                    loggerService.ConsoleLog(message);
                    loggerService.FileLog(CoinAutoTradeLogDirectoryPath, message);
                    return;
                }
                
                investAmount -= cancelJson.GetExecutedVolume() * price;
            }
            else
            {
                investAmount -= orderJson.GetExecutedVolume() * price;
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
        if (Market == null)
            return;

        var marketBalanceResponse= await Market.RequestBalance(coinTradeData.Symbol);
        var balance = marketBalanceResponse?.GetMarketBalance(coinTradeData.Symbol)?.Balance;

        if (balance == null)
            return;
        
        var loggerService = Client.LoggerService;
        var message = $"{coinTradeData.MarketCode} {nameof(SellTradeAsync)}";
        loggerService.FileLog(CoinAutoTradeLogDirectoryPath, message);
        loggerService.ConsoleLog(message);
        
        double sellPrice;
        
        do
        {
            var orderBookResponse = await Market.RequestMarketOrderBook(coinTradeData.MarketCode);
            var orderBooks = orderBookResponse?.GetBidOrderBooks(coinTradeData.MarketCode);
            if (orderBooks is not { Count: > 0 })
            {
                message = $"{orderBooks} is null";
                loggerService.ConsoleLog(message);
                loggerService.FileLog(CoinAutoTradeLogDirectoryPath, message);
                return;
            }
            
            var (price, size) = orderBooks[0];
            if (size <= 0 || price * size < CoinTradeData.MinInvestAmount)
            {
                if (orderBooks.Count < 2)
                {
                    message = $"{orderBooks} {nameof(size)} is 0";
                    loggerService.ConsoleLog(message);
                    loggerService.FileLog(CoinAutoTradeLogDirectoryPath, message);
                    return;
                }
                
                (price, size) = orderBooks[1];
            }

            var volume = Math.Min(size, balance.GetValueOrDefault());
            if (volume <= 0)
            {
                message = $"{volume} is zero";
                loggerService.ConsoleLog(message);
                loggerService.FileLog(CoinAutoTradeLogDirectoryPath, message);
                return;
            }

            sellPrice = price;
            
            var sellResponse = await Market.RequestSell(coinTradeData.MarketCode, volume, price);
            var uuid = sellResponse?.Result?.Uuid;
            if (string.IsNullOrEmpty(uuid))
            {
                message = $"{nameof(sellResponse)} {uuid} is null";
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
                
            if (orderJson.GetState() != EMarketOrderState.Done)
            {
                var cancelOrderResponse = await Market.RequestCancelOrder(uuid);
                var cancelJson = cancelOrderResponse?.Result;
                if (cancelJson == null)
                {
                    message = $"{cancelJson} is null";
                    loggerService.ConsoleLog(message);
                    loggerService.FileLog(CoinAutoTradeLogDirectoryPath, message);
                    return;
                }
                
                balance -= cancelJson.GetExecutedVolume();
            }
            else
            {
                balance -= orderJson.GetExecutedVolume();
            }
                
            message = $"{nameof(SellTradeAsync)} [{Client.MarketType}] {coinTradeData.MarketCode} price = {price} amount = {orderJson.GetExecutedVolume()}";
            await loggerService.TelegramLogAsync(message);
            loggerService.ConsoleLog(message);
            loggerService.FileLog(CoinAutoTradeLogDirectoryPath, $"{message}");
            
        } while (balance.GetValueOrDefault() > 0);

        if (coinTradeData.RebalancingCount < coinTradeData.RebalancingMaxCount)
        {
            coinTradeData.BuyPrice = sellPrice * (1 - coinTradeData.RoundSellRate * CoinTradeData.RebalancingRate);
            coinTradeData.SellPrice = sellPrice * (1 - coinTradeData.RoundSellRate);
            coinTradeData.RebalancingCount++;
        }
        else
        {
            coinTradeData.State = ECoinTradeState.Stop;
        }
        
        await Client.RequestInnerAddOrUpdateCoinTradeDataAsync("Sell", coinTradeData);
    }
}