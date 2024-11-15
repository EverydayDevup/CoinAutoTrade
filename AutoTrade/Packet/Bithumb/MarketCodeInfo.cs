using Newtonsoft.Json;
using RestSharp;

namespace AutoTrade.Packet.Bithumb;

public struct MarketCodeInfo
{
    public string market { get; set; }
}

public class MarketCodeResponse : IResponse
{
    public bool IsSuccess { get; set; }
    public string[]? MarketCodes { get; private set; }

    public void Parse(RestResponse res)
    {
        if (res.ResponseStatus != ResponseStatus.Completed ||
            string.IsNullOrEmpty(res.Content))
        {
            IsSuccess = false;
            return;
        }
        
        var marketCodeList = JsonConvert.DeserializeObject<List<MarketCodeInfo>>(res.Content);
        if (marketCodeList == null)
        {
            IsSuccess = false;
            return;
        }

        MarketCodes = new string[marketCodeList.Count];
        for (var i = 0; i < marketCodeList.Count; i++)
            MarketCodes[i] = marketCodeList[i].market;

        IsSuccess = true;
    }
}