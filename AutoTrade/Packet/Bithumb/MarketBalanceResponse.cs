using Newtonsoft.Json;
using RestSharp;

namespace AutoTrade.Packet.Bithumb;

public struct MarketBalanceResponseJson
{
    [JsonProperty("currency")]
    public string Currency {get; set;}
    [JsonProperty("balance")]
    public string Balance { get; set; }
}

public class MarketBalanceResponse : Response<List<MarketBalanceResponseJson>, Dictionary<string, double>>
{
    protected override void Parse(List<MarketBalanceResponseJson> content)
    {
        Result = new();
        foreach (var marketBalance in content)
        {
            var balance = double.TryParse(marketBalance.Balance, out var balanceResult) ? balanceResult : 0;
            Result.TryAdd(marketBalance.Currency, balance);
        }
    }
}
