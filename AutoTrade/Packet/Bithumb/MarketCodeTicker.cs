using Newtonsoft.Json;
using RestSharp;

namespace AutoTrade.Packet.Bithumb;

public struct MarketCodeTicker
{
    public double trade_price { get; set; }
}

public class MarketCodeTickerResponse : IResponse
{
    public bool IsSuccess { get; set; }
    public double TradePrice { get; set; }

    public void Parse(RestResponse res)
    {
        if (res.ResponseStatus != ResponseStatus.Completed ||
            string.IsNullOrEmpty(res.Content))
        {
            IsSuccess = false;
            return;
        }
        
        var marketCodeTickerList = JsonConvert.DeserializeObject<List<MarketCodeTicker>>(res.Content);
        if (marketCodeTickerList == null || marketCodeTickerList.Count <= 0)
        {
            IsSuccess = false;
            return;
        }
        
        TradePrice = marketCodeTickerList[0].trade_price;
        IsSuccess = true;
    }
}