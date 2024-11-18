using Newtonsoft.Json;

namespace AutoTrade.Packet.Bithumb;

public class MarketSellResponseJson
{
    [JsonProperty("uuid")]
    public string Uuid { get; set; }
}

public class MarketSellResponse : Response<MarketSellResponseJson, string>
{
    protected override void Parse(MarketSellResponseJson content)
    {
        Result = content.Uuid;
    }
}