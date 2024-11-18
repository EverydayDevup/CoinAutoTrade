using Newtonsoft.Json;

namespace AutoTrade.Packet.Bithumb;

public class MarketCheckOrderResponseJson
{
    [JsonProperty("uuid")]
    public string Uuid { get; set; }
}

public class MarketCheckOrderResponse : Response<MarketCheckOrderResponseJson, bool>
{
    protected override void Parse(MarketCheckOrderResponseJson content)
    {
        Result = !string.IsNullOrEmpty(content.Uuid);
    }
}