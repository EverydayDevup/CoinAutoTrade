using Newtonsoft.Json;

namespace AutoTrade.Packet.Bithumb;

public class MarketCancelOrderResponseJson
{
    [JsonProperty("uuid")]
    public string Uuid { get; set; }
}

public class MarketCancelOrderResponse : Response<MarketCancelOrderResponseJson, bool>
{
    protected override void Parse(MarketCancelOrderResponseJson content)
    {
        Result = !string.IsNullOrEmpty(content.Uuid);
    }
}