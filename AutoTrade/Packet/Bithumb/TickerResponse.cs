using Newtonsoft.Json;

namespace AutoTrade.Packet.Bithumb;

public class TickerResponseJson
{
    [JsonProperty("trade_price")]
    public double TradePrice { get; set; }
}

public class TickerResponse : Response<List<TickerResponseJson>, double>
{
    protected override void Parse(List<TickerResponseJson>? content)
    {
        if (content == null || content.Count < 1)
            return;
        
        Result = content[0].TradePrice;
    }
}