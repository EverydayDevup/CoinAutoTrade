using AutoTrade.Config;
using AutoTrade.Market;

namespace AutoTrade.Logic;

public static class CoinTrade
{
    private static Dictionary<string, CoinTradeProcess> dicCoinTradeProcess = new();
    private const int Delay = 1000 / 100;
    
    public static async Task Trade(IMarket market, CoinConfig coinConfig)
    {
        if (!dicCoinTradeProcess.TryGetValue(coinConfig.MarketCode, out var process))
            dicCoinTradeProcess.Add(coinConfig.MarketCode, process = new(market, coinConfig));

        while (process.IsTrade)
        {
            await process.Trade();
            await Task.Delay(Delay);
        }
    }
}

public class CoinTradeProcess
{
    public bool IsTrade { get; set; }
    
    private readonly IMarket market;
    private readonly CoinConfig coinConfig;

    public CoinTradeProcess(IMarket market, CoinConfig coinConfig)
    {
        IsTrade = true;
        this.market = market;
        this.coinConfig = coinConfig;
    }
    
    public async Task Trade()
    {
        var tradePrice = await market.RequestTicker(coinConfig.MarketCode);
        Console.WriteLine($"3.1 {nameof(Trade)} {nameof(coinConfig.MarketCode)} : {coinConfig.MarketCode} - {nameof(tradePrice)} : {tradePrice}");
        
        
    }
}