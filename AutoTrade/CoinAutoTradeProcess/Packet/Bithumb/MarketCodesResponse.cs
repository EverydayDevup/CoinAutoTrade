using Newtonsoft.Json;

namespace AutoTrade.Packet.Bithumb;

public struct MarketCodeResponseJson
{
    [JsonProperty("market")]
    public string MarketCode { get; set; }
}

public class MarketCodesResponse : Response<List<MarketCodeResponseJson>, string[]>
{
    protected override void Parse(List<MarketCodeResponseJson> content)
    {
        Result = new string[content.Count];
        for (var i = 0; i < content.Count; i++)
            Result[i] = content[i].MarketCode;
    }
}