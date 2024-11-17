using Newtonsoft.Json;
using RestSharp;

namespace AutoTrade.Packet.Bithumb;

public struct MarketAccount
{
    public string currency {get; set;}
    public string balance { get; set; }
}

public class MarketAccountResponse : IResponse
{
    public bool IsSuccess { get; set; }
    public Dictionary<string, double> DicBalances { get; private set; } = new(); 

    public void Parse(RestResponse res)
    {
        if (res.ResponseStatus != ResponseStatus.Completed ||
            string.IsNullOrEmpty(res.Content))
        {
            IsSuccess = false;
            return;
        }
        
        var marketAccountList = JsonConvert.DeserializeObject<List<MarketAccount>>(res.Content);
        if (marketAccountList == null)
        {
            IsSuccess = false;
            return;
        }
        
        foreach (var marketAccount in marketAccountList)
        {
            var balance = double.TryParse(marketAccount.balance, out var balanceResult) ? balanceResult : 0;
           DicBalances.TryAdd(marketAccount.currency, balance);
        }
        
        IsSuccess = true;
    }
}