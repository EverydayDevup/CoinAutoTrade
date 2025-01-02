using Newtonsoft.Json;

namespace CoinAutoTradeProcess;

public class MarketOrderBooksUnitJson
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

public class MarketOrderBooksJson
{
    [JsonProperty("market")]
    public string? MarketCode { get; set; }
    
    [JsonProperty("orderbook_units")]
    public List<MarketOrderBooksUnitJson>? MarketOrderBooksUnits { get; set; }
}

public class MarketOrderBookResponse : Response<List<MarketOrderBooksJson>>
{
    public List<(double, double)>? GetAskOrderBooks(string marketCode)
    {
        var orderBooksUnits = Result?.Find(x => x.MarketCode == marketCode);
        return orderBooksUnits?.MarketOrderBooksUnits?.ConvertAll(elem => (elem.AskPrice, elem.AskSize));
    }
    
    public List<(double, double)>? GetBidOrderBooks(string marketCode)
    {
        var orderBooksUnits = Result?.Find(x => x.MarketCode == marketCode);
        return orderBooksUnits?.MarketOrderBooksUnits?.ConvertAll(elem => (elem.BidPrice, elem.BidSize));
    }
}