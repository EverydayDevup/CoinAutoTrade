using Newtonsoft.Json;

namespace CoinAutoTradeProcess;

public class MarketCodeJson
{
    [JsonProperty("market")]
    public string? MarketCode { get; set; }

    public override string ToString()
    {
        if (string.IsNullOrEmpty(MarketCode))
            return string.Empty;
        
        return MarketCode;
    }
}

public class MarketCodesResponse : Response<List<MarketCodeJson>>
{
    public bool IsExist(string marketCode)
    {
        return Result?.Any(m => m.MarketCode == marketCode) ?? false;
    }

    public override string ToString()
    {
        if (Result == null)
            return string.Empty;
        
        return string.Join(",", Result.ToList());
    }
}