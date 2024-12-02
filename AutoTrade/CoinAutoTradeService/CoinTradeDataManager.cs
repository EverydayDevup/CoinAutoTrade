using Newtonsoft.Json;
using SharedClass;

namespace CoinAutoTrade;

public class CoinTradeDataManager
{
    private LoggerService.LoggerService _loggerService = null;
    private readonly Dictionary<string, Dictionary<string, CoinTradeData>> _dicMarketCoinTradeData = new();
    private readonly string _coinTradeDataDirectoryName = "CoinTradeData";
    private readonly string _coinConfigFileName = "CoinConfig.json";

    private string GetCoinTradeDataListFilePath(string marketName)
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        return Path.Combine(currentDirectory, marketName, _coinTradeDataDirectoryName, _coinConfigFileName);
    }
    
    public bool Load(string marketName, LoggerService.LoggerService loggerService)
    {
        _loggerService ??= loggerService;
        
        var coinTradeDataListFilepath = GetCoinTradeDataListFilePath(marketName);
        
        if (!File.Exists(coinTradeDataListFilepath))
        {
            _loggerService.ConsoleError($"[{nameof(CoinTradeDataManager)}] Not found {nameof(coinTradeDataListFilepath)} : {coinTradeDataListFilepath}");
            return false;
        }
        
        var coinTradeDataListJson = File.ReadAllText(coinTradeDataListFilepath);
        var coinTradeDataList = JsonConvert.DeserializeObject<List<CoinTradeData>>(coinTradeDataListJson);

        if (coinTradeDataList == null)
        {
            _loggerService.ConsoleError($"[{nameof(CoinTradeDataManager)}] Not found {nameof(coinTradeDataList)}");
            return false;
        }

        if (!_dicMarketCoinTradeData.TryGetValue(marketName, out var dicCoinTradeData))
        {
            dicCoinTradeData = new Dictionary<string, CoinTradeData>();
            _dicMarketCoinTradeData.Add(marketName, dicCoinTradeData);
        }
        
        dicCoinTradeData.Clear();
        foreach (var coinTradeData in coinTradeDataList)
        {
            var errorMessage = coinTradeData.GetValidMessage();
            if (!string.IsNullOrEmpty(errorMessage))
            {
                _loggerService.ConsoleError($"[{nameof(CoinTradeDataManager)}] {nameof(coinTradeData)} {coinTradeData.Symbol} : {errorMessage}");
                return false;
            }
            
            dicCoinTradeData.TryAdd(coinTradeData.Symbol, coinTradeData);
        }

        return true;
    }

    private void Save(string marketName)
    {
        if (!_dicMarketCoinTradeData.TryGetValue(marketName, out var dicCoinTradeData))
        {
            _loggerService.ConsoleError($"[{nameof(CoinTradeDataManager)}] {nameof(marketName)} {marketName} not found");
            return;
        }
        
        var coinTradeDataListFilepath = GetCoinTradeDataListFilePath(marketName);
        var coinTradeDataList = dicCoinTradeData.Values.ToList();
        var coinTradeDataListJson = JsonConvert.SerializeObject(coinTradeDataList);
        
        File.WriteAllText(coinTradeDataListFilepath, coinTradeDataListJson);
    }

    public void Remove(string marketName, string symbol)
    {
        if (!_dicMarketCoinTradeData.TryGetValue(marketName, out var dicCoinTradeData))
            return;
        
        if (!dicCoinTradeData.ContainsKey(symbol))
            return;
            
        dicCoinTradeData.Remove(symbol);
        Save(marketName);
    }

    public void Update(string marketName, CoinTradeData coinTradeData)
    {
        if (!_dicMarketCoinTradeData.TryGetValue(marketName, out var dicCoinTradeData))
            return;
        
        dicCoinTradeData.Remove(coinTradeData.Symbol);
        dicCoinTradeData.Add(coinTradeData.Symbol, coinTradeData);
    }
}