using AutoTrade.Config;
using AutoTrade.Market;
using Newtonsoft.Json;

namespace AutoTrade.Logic;

public static class CoinConfigFactory
{
    /// <summary>
    /// 코인 설정과 관련된 정보를 가져옴
    /// </summary>
    public static async Task<List<CoinConfig>?> CreateAsync(IMarket market)
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var coinConfigFilepath = Path.Combine(currentDirectory, PathConfig.DataDirectoryName, PathConfig.CoinConfigFileName);
        
        if (!File.Exists(coinConfigFilepath))
        {
            Console.WriteLine($"not found {nameof(coinConfigFilepath)} : {coinConfigFilepath}");
            return null;
        }

        var coinConfigJson = await File.ReadAllTextAsync(coinConfigFilepath);
        var coinConfigList = JsonConvert.DeserializeObject<List<CoinConfig>>(coinConfigJson);

        if (coinConfigList is not { Count: > 0 })
        {
            Console.WriteLine($"{nameof(coinConfigList)} is empty");
            return null;
        }

        var marketCodes = await market.RequestMarketCodes();
        if (marketCodes == null)
        {
            Console.WriteLine($"{nameof(market.RequestMarketCodes)} returned null");
            return null;
        }

        foreach (var coinConfig in coinConfigList)
        {
            if (marketCodes.Contains(coinConfig.MarketCode))
            {
                if (coinConfig.SellRate > coinConfig.BuyRate)
                {
                    Console.WriteLine($"{nameof(coinConfig.SellRate)} is over {coinConfig.BuyRate}");
                    return null;
                }
                
                continue;
            }
            
            Console.WriteLine($"{nameof(coinConfig.MarketCode)} is not supported : {coinConfig.MarketCode}");
            return null;
        }
        
        foreach (var coinConfig in coinConfigList)
        {
            Console.WriteLine(coinConfig.ToLog());
        }

        return coinConfigList;
    }
}