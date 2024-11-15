using AutoTrade.Packet;
using AutoTrade.Packet.Bithumb;
using RestSharp;

namespace AutoTrade.Market;

public class Bithumb : IMarket
{
    public async Task<MarketOrderInfo?> RequestOrderbook(string marketCode)
    {
        var options = new RestClientOptions($"https://api.bithumb.com/v1/orderbook?markets={marketCode}");
        var client = new RestClient(options);
        var request = new RestRequest("");
        request.AddHeader("accept", "application/json");
        var res = await PacketHelper.RequestAsync<MarketOrderbookResponse>(client, request);
        if (res is { IsSuccess: true })
            return res.MarketOrderInfo;
        
        Console.WriteLine($"{nameof(RequestOrderbook)} failed");
        return null;
    }

    public async Task<double> RequestTicker(string marketCode)
    {
        var options = new RestClientOptions($"https://api.bithumb.com/v1/ticker?markets={marketCode}");
        var client = new RestClient(options);
        var request = new RestRequest("");
        request.AddHeader("accept", "application/json");
        
        var res = await PacketHelper.RequestAsync<MarketCodeTickerResponse>(client, request);
        if (res is { IsSuccess: true })
            return res.TradePrice;
        
        Console.WriteLine($"{nameof(RequestTicker)} failed");
        return 0;
    }

    public async Task<string[]?> RequestMarketCodes()
    {
        var options = new RestClientOptions("https://api.bithumb.com/v1/market/all?isDetails=false");
        var client = new RestClient(options);
        var request = new RestRequest("");
        request.AddHeader("accept", "application/json");

        var res = await PacketHelper.RequestAsync<MarketCodeResponse>(client, request);
        if (res is { IsSuccess: true })
            return res.MarketCodes;
        
        Console.WriteLine($"{nameof(RequestMarketCodes)} failed");
        return null;
    }
}