using System.Globalization;
using Newtonsoft.Json;
using RestSharp;

namespace AutoTrade.Packet.Bithumb;

public struct MarkBuyOrder
{
    public string uuid { get; set; }
}

public struct MarketOrder
{
    public string market { get; set; }
    public string side { get; set; }
    public string volume { get; set; }
    public string price { get; set; }
    public string ord_type { get; set; }

    public MarketOrder(string market, string side, double volume, double price, string ord_type)
    {
        this.market = market;
        this.side = side;
        this.volume = volume.ToString(CultureInfo.InvariantCulture);
        this.price = price.ToString(CultureInfo.InvariantCulture);
        this.ord_type = ord_type;
    }
}

public class MarkBuyOrderResponse : IResponse
{
    public bool IsSuccess { get; set; }
    public string Uuid { get; set; }

    public void Parse(RestResponse res)
    {
        if (res.ResponseStatus != ResponseStatus.Completed ||
            string.IsNullOrEmpty(res.Content))
        {
            IsSuccess = false;
            return;
        }
        
        var marketBuyOrder = JsonConvert.DeserializeObject<MarkBuyOrder?>(res.Content);
        if (marketBuyOrder == null)
        {
            IsSuccess = false;
            return;
        }

        Uuid = marketBuyOrder.Value.uuid;
        IsSuccess = true;
    }
}