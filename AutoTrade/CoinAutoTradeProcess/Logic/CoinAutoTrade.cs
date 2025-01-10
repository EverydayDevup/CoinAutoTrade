using SharedClass;

namespace CoinAutoTradeProcess;

public class CoinAutoTrade(IMarket? market, CoinAutoTradeProcessClient client)
{
    private IMarket? Market { get; } = market;
    private const int Delay = 5;
    private bool IsRunning { get; set; }

    private List<CoinTradeData>? _coinTradeDataList;
    private List<CoinTradeData>? _runCoinTradeDataList;
    private CoinAutoTradeProcessClient Client { get; } = client;
    private string CoinAutoTradeLogDirectoryPath => Path.Combine(nameof(CoinAutoTrade), Client.MarketType.ToString());
    
    private bool _isCoinTradeDataListRefresh = false;

    public void Reload(List<CoinTradeData>? coinTradeDataList)
    {
        _coinTradeDataList = coinTradeDataList;
        _isCoinTradeDataListRefresh = true;
        
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

            if (_isCoinTradeDataListRefresh)
            {
                _runCoinTradeDataList = new List<CoinTradeData>(_coinTradeDataList.Count);
                _runCoinTradeDataList.AddRange(_coinTradeDataList);
                _isCoinTradeDataListRefresh = false;
            }
            
            if (_runCoinTradeDataList == null || _runCoinTradeDataList.Count == 0)
            {
                await Task.Delay(Delay);
                continue;
            }
            
            foreach (var coinTradeData in _runCoinTradeDataList)
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
            case ECoinTradeState.Rebalancing:
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
           await Client.RequestInnerAddOrUpdateCoinTradeDataAsync("ready to progress", coinTradeData);

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
                await Client.RequestInnerAddOrUpdateCoinTradeDataAsync("init price setting", coinTradeData);
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

        switch (coinTradeData.State)
        {
            // 일반적인 상황에서는 Buy Price의 값이 더 큼
            case ECoinTradeState.Progress:
            {
                if (ticker >= coinTradeData.BuyPrice)
                    await BuyTradeAsync(coinTradeData);
                break;
            }
            // 리밸런싱 상태일 때는 현재가 보다 낮아질 때 구매를 하기 때문에, 조건 체크가 변경됨
            case ECoinTradeState.Rebalancing:
            {
                if (ticker <= coinTradeData.BuyPrice)
                    await BuyTradeAsync(coinTradeData);
                break;
            }
        }
        
        if (ticker <= coinTradeData.SellPrice)
            await SellTradeAsync(coinTradeData);
    }

    private async Task<double> GetBalanceAsync(string symbol)
    {
        if (Market == null)
            return 0;
        
        // 현재 원화 보유량을 가져옴
        var balanceResponse = await Market.RequestBalance(symbol);
        var marketBalanceJson = balanceResponse?.GetMarketBalance(symbol);
        if (marketBalanceJson == null)
            return 0;

        return marketBalanceJson.Balance;
    }

    private async Task BuyTradeAsync(CoinTradeData coinTradeData)
    {
        if (Market == null)
            return;

        var investAmount = coinTradeData.InvestRoundAmount;
        double buyPrice = 0;

        var loggerService = Client.LoggerService;
        var message = $"{coinTradeData.MarketCode} {nameof(BuyTradeAsync)}";
        loggerService.FileLog(CoinAutoTradeLogDirectoryPath, message);
        loggerService.ConsoleLog(message);

        // 현재 원화 보유량을 가져옴
        var krwBalance = await GetBalanceAsync("KRW");
        if (krwBalance < CoinTradeData.TotalInvestAmount)
            return;
            
        while (investAmount >= CoinTradeData.MinInvestAmount && investAmount < krwBalance)
        {
            // 현재 매도 주문을 가져옴
            var orderBookResponse = await Market.RequestMarketOrderBook(coinTradeData.MarketCode);
            var orderBooks = orderBookResponse?.GetAskOrderBooks(coinTradeData.MarketCode);
            if (orderBooks is not { Count: > 0 })
            {
                message = $"{orderBooks} is null";
                loggerService.ConsoleLog(message);
                loggerService.FileLog(CoinAutoTradeLogDirectoryPath, message);
                return;
            }

            (buyPrice, _) = orderBooks[0];
            
            // 매수 주문을 넣음
            var volume = investAmount / buyPrice;
            var buyResponse = await Market.RequestBuy(coinTradeData.MarketCode, volume, buyPrice);
            var uuid = buyResponse?.Result?.Uuid;
            if (string.IsNullOrEmpty(uuid))
            {
                message = $"{nameof(buyResponse)} {uuid} is null";
                loggerService.ConsoleLog(message);
                loggerService.FileLog(CoinAutoTradeLogDirectoryPath, message);
                return;
            }

            await Task.Delay(Delay);
            
            // 구입이 되었는지 확인하기 위해 주문 정보를 조회
            var orderResponse = await Market.RequestOrder(uuid);
            var orderJson = orderResponse?.Result;
            if (orderJson == null)
            {
                message = $"{orderJson} is null";
                loggerService.ConsoleLog(message);
                loggerService.FileLog(CoinAutoTradeLogDirectoryPath, message);
                return;
            }

            // 주문 처리가 안되었다면 취소 
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
            }
            
            var preKrwBalance = krwBalance;
            krwBalance = await GetBalanceAsync("KRW");
            
            investAmount -= Math.Min(0, preKrwBalance - krwBalance);
            var coinBalance = await GetBalanceAsync(coinTradeData.Symbol);

            message = $"{nameof(BuyTradeAsync)} [{Client.MarketType}] {coinTradeData.MarketCode} " +
                      $"{nameof(buyPrice)} = {buyPrice} amount = {orderJson.GetExecutedVolume()} krw = {krwBalance} {coinTradeData.Symbol} = {coinBalance}";
            
            loggerService.ConsoleLog(message);
            loggerService.FileLog(CoinAutoTradeLogDirectoryPath, $"{message}");
            await loggerService.TelegramLogAsync(message);
        }

        if (buyPrice <= 0)
        {
            message = $"{nameof(buyPrice)} is zero / {nameof(investAmount)} = {investAmount} {nameof(krwBalance)} = {krwBalance}";
            loggerService.FileLog(CoinAutoTradeLogDirectoryPath, $"{message}");
            coinTradeData.State = ECoinTradeState.Stop;
            await Client.RequestInnerAddOrUpdateCoinTradeDataAsync("Buy Error", coinTradeData);
            return;
        }
        
        coinTradeData.BuyPrice = buyPrice * (1 + coinTradeData.RoundBuyRate);
        coinTradeData.SellPrice = buyPrice * (1 - coinTradeData.RoundSellRate);
        coinTradeData.BuyCount++;
        
        if (coinTradeData.State == ECoinTradeState.Rebalancing)
            coinTradeData.State = ECoinTradeState.Progress;
        
        await Client.RequestInnerAddOrUpdateCoinTradeDataAsync("Buy", coinTradeData);
    }

    private async Task SellTradeAsync(CoinTradeData coinTradeData)
    {
        if (Market == null)
            return;

        var coinBalance = await GetBalanceAsync(coinTradeData.Symbol);
        if (coinBalance <= 0)
        {
            coinTradeData.State = ECoinTradeState.Stop;
            await Client.RequestInnerAddOrUpdateCoinTradeDataAsync("Sell", coinTradeData);
            return;
        }
        
        var loggerService = Client.LoggerService;
        var message = $"{coinTradeData.MarketCode} {nameof(SellTradeAsync)}";
        loggerService.FileLog(CoinAutoTradeLogDirectoryPath, message);
        loggerService.ConsoleLog(message);

        double sellPrice = 0;

        while (coinBalance > 0)
        {
            // 매수 주문 가져오기
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
            // 최소 거래 비용보다 매수 주문이 작은 경우, 다음 매수 주문까지 확장함
            if (price * size < CoinTradeData.MinInvestAmount)
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

            var volume = Math.Min(size, coinBalance);
            if (volume <= 0)
            {
                message = $"{volume} is zero";
                loggerService.ConsoleLog(message);
                loggerService.FileLog(CoinAutoTradeLogDirectoryPath, message);
                return;
            }

            sellPrice = price;

            // 주문할 수 있는 금액이 최소 투자 금액보다 작다면 매도를 하지 못함
            if (sellPrice * volume < CoinTradeData.MinInvestAmount)
            {
                coinBalance = 0;
                continue;
            }
            
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
            }

            var krwBalance = await GetBalanceAsync("KRW");
            coinBalance = await GetBalanceAsync(coinTradeData.Symbol);
            
            message = $"{nameof(SellTradeAsync)} [{Client.MarketType}] {coinTradeData.MarketCode} price = {price} amount = {orderJson.GetExecutedVolume()} " +
                      $"krw = {krwBalance} {coinTradeData.Symbol} = {coinBalance}";
            
            loggerService.ConsoleLog(message);
            loggerService.FileLog(CoinAutoTradeLogDirectoryPath, $"{message}");
            await loggerService.TelegramLogAsync(message);
        }
        
        if (sellPrice <= 0)
        {
            message = $"{nameof(sellPrice)} is zero / {nameof(coinBalance)} = {coinBalance}";
            loggerService.FileLog(CoinAutoTradeLogDirectoryPath, $"{message}");
            coinTradeData.State = ECoinTradeState.Stop;
            await Client.RequestInnerAddOrUpdateCoinTradeDataAsync("Sell Error", coinTradeData);
            return;
        }

        if (coinTradeData.RebalancingCount < coinTradeData.RebalancingMaxCount)
        {
            coinTradeData.BuyPrice = sellPrice * (1 - coinTradeData.RoundSellRate * CoinTradeData.RebalancingRate);
            coinTradeData.SellPrice = sellPrice * (1 - coinTradeData.RoundSellRate);
            coinTradeData.RebalancingCount++;
            coinTradeData.State = ECoinTradeState.Rebalancing;
        }
        else
        {
            coinTradeData.State = ECoinTradeState.Stop;
        }
        
        await Client.RequestInnerAddOrUpdateCoinTradeDataAsync("Sell", coinTradeData);
    }
}