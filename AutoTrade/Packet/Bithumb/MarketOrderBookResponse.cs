using AutoTrade.Packet.Common;
using Newtonsoft.Json;

namespace AutoTrade.Packet.Bithumb;

public class MarketOrderBooksUnitResponseJson
{
    [JsonProperty("ask_price")]
    public double AskPrice { get; set; }
    [JsonProperty("bid_price")]
    public double BidPrice { get; set; }
    [JsonProperty("ask_size")]
    public double AskSize { get; set; }
    [JsonProperty("bid_size")]
    public double BidSize { get; set; }
}

public class MarketOrderBooksResponseJson
{
    [JsonProperty("orderbook_units")]
    public List<MarketOrderBooksUnitResponseJson> MarketOrderBooksUnits { get; set; } = new();
}

public class MarketOrderBookResponse : Response<List<MarketOrderBooksResponseJson>, MarketOrderBook>
{
    protected override void Parse(List<MarketOrderBooksResponseJson> content)
    {
        var marketOrderBooksUnits = content[0].MarketOrderBooksUnits;
        
        Result = new MarketOrderBook
        {
            BuyOrders = new Common.MarketOrder[marketOrderBooksUnits.Count],
            SellOrders = new Common.MarketOrder[marketOrderBooksUnits.Count]
        };

        for (var i = 0; i < marketOrderBooksUnits.Count; i++)
        {
            Result.BuyOrders[i] = new Common.MarketOrder(marketOrderBooksUnits[i].AskPrice, marketOrderBooksUnits[i].AskSize);
            Result.SellOrders[i] = new Common.MarketOrder(marketOrderBooksUnits[i].BidPrice, marketOrderBooksUnits[i].BidSize);
        }
    }
}