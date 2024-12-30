using Newtonsoft.Json;
using RestSharp;

namespace AutoTrade.Packet.Bithumb;

public class MarketBuyResponseJson
{
    [JsonProperty("uuid")]
    public string Uuid { get; set; }
}


public class MarketBuyResponse : Response<MarketBuyResponseJson, string>
{
    protected override void Parse(MarketBuyResponseJson content)
    {
        Result = content.Uuid;
    }
}