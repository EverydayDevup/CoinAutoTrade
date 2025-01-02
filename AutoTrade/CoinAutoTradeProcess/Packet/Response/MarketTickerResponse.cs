using Newtonsoft.Json;

namespace CoinAutoTradeProcess;

public class MarketTickerJson
{
    [JsonProperty("market")]
    public string? MarketCode { get; set; }
    
    [JsonProperty("trade_price")]
    public string? TradePrice { get; set; }

    public double Price
    {
        get
        {
            if (string.IsNullOrEmpty(TradePrice))
                return 0;
            
            return double.Parse(TradePrice);
        }
    }
}


public class MarketTickerResponse : Response<List<MarketTickerJson>>
{
    public MarketTickerJson? GetMarketTickerJson(string marketCode)
    {
        return Result?.Find(x => x.MarketCode == marketCode);
    }
}