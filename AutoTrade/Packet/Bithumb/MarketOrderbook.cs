using Newtonsoft.Json;
using RestSharp;

namespace AutoTrade.Packet.Bithumb;

public struct OrderbooksUnit
{
    public double ask_price { get; set; }
    public double bid_price { get; set; }
    public double ask_size { get; set; }
    public double bid_size { get; set; }
}

public struct MarketOrderbook
{
    public List<OrderbooksUnit> orderbook_units { get; set; }
}

public class MarketOrderbookResponse : IResponse
{
    public bool IsSuccess { get; set; }
    
    public MarketOrderInfo MarketOrderInfo { get; private set; }

    public void Parse(RestResponse res)
    {
        if (res.ResponseStatus != ResponseStatus.Completed ||
            string.IsNullOrEmpty(res.Content))
        {
            IsSuccess = false;
            return;
        }
            
        var marketOrderbookList = JsonConvert.DeserializeObject<List<MarketOrderbook>>(res.Content);
        if (marketOrderbookList == null || marketOrderbookList.Count == 0)
        {
            IsSuccess = false;
            return;
        }

        var marketOrderbook = marketOrderbookList[0];
        var orderbooks = marketOrderbook.orderbook_units;

        MarketOrderInfo = new MarketOrderInfo();
        MarketOrderInfo.BuyOrderbooks = new Orderbook[orderbooks.Count];
        MarketOrderInfo.SellOrderbooks = new Orderbook[orderbooks.Count];
        
        for (var i = 0; i < orderbooks.Count; i++)
        {
            MarketOrderInfo.BuyOrderbooks[i] = new Orderbook(orderbooks[i].ask_price, orderbooks[i].ask_size);
            MarketOrderInfo.SellOrderbooks[i] = new Orderbook(orderbooks[i].bid_price, orderbooks[i].bid_size);
        }
        
        IsSuccess = true;
    }
}