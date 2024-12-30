using Newtonsoft.Json;

namespace AutoTrade.Packet.Bithumb;

public class MarketCheckOrderResponseJson
{
    [JsonProperty("uuid")]
    public string Uuid { get; set; }
    [JsonProperty("state")]
    public string State { get; set; }
}

public class MarketCheckOrderResponse : Response<MarketCheckOrderResponseJson, bool>
{
    protected override void Parse(MarketCheckOrderResponseJson content)
    {
        Result = !string.IsNullOrEmpty(content.Uuid) && content.State == "wait";
    }
}