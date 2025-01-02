using Newtonsoft.Json;

namespace CoinAutoTradeProcess;

public class MarketBalanceJson
{
    [JsonProperty("currency")] 
    public string? Symbol { get; set; }
    [JsonProperty("balance")] 
    public double Balance { get; set; }
}

public class MarketBalanceResponse : Response<List<MarketBalanceJson>>
{
    public MarketBalanceJson? GetMarketBalance(string symbol)
    {
        return Result?.Find((elem) => elem.Symbol == symbol);
    }
}