using Newtonsoft.Json;

namespace CoinAutoTradeProcess;

public class MarketCodeJson
{
    [JsonProperty("market")]
    public string? MarketCode { get; set; }
}

public class MarketCodesResponse : Response<List<MarketCodeJson>>
{
    public bool IsExist(string marketCode)
    {
        return Result?.Any(m => m.MarketCode == marketCode) ?? false;
    }
}