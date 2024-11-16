using Newtonsoft.Json;
using RestSharp;

namespace AutoTrade.Packet.Bithumb;

public class MarketCheckOrder
{
    public string uuid { get; set; }
}

public class MarketCheckOrderResponse : IResponse
{
    public bool IsSuccess { get; set; }
    public bool IsExist { get; set; }
    
    public void Parse(RestResponse res)
    {
        IsExist = false;
        
        if (res.ResponseStatus != ResponseStatus.Completed ||
            string.IsNullOrEmpty(res.Content))
        {
            IsSuccess = false;
            return;
        }
        
        var marketCheckOrder = JsonConvert.DeserializeObject<MarketCheckOrder>(res.Content);
        if (marketCheckOrder == null)
        {
            IsSuccess = false;
            return;
        }
        
        IsSuccess = true;
        IsExist = !string.IsNullOrEmpty(marketCheckOrder.uuid);
    }
}