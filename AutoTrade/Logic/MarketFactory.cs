using AutoTrade.Config;
using AutoTrade.Market;
using Newtonsoft.Json;

namespace AutoTrade.Logic;

public static class MarketFactory
{
    private static readonly Dictionary<string, Func<IMarket>> DicMarketFactory = new ()
    {
        {"Bithumb", () => new Bithumb()}
    };
    
    public static IMarket? Create()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var configFilepath = Path.Combine(currentDirectory, PathConfig.DataDirectoryName, PathConfig.MarketConfigFileName);

        if (!File.Exists(configFilepath))
        {
            Console.WriteLine($"not found {nameof(configFilepath)} : {configFilepath}");
            return null;
        }
        
        var configJson = File.ReadAllText(configFilepath);
        var config = JsonConvert.DeserializeObject<MarketConfig>(configJson);

        if (string.IsNullOrEmpty(config.Market))
        {
            Console.WriteLine($"not found {nameof(config.Market)}");
            return null;
        }

        Console.WriteLine(config.ToLog());

        if (!DicMarketFactory.TryGetValue(config.Market, out var factory))
        {
            Console.WriteLine($"not supported market {config.Market}");
            return null;
        }

        return factory.Invoke();
    }
}