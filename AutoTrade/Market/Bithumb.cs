using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using AutoTrade.Logic;
using AutoTrade.Packet.Bithumb;
using AutoTrade.Packet.Common;
using RestSharp;

namespace AutoTrade.Market;

public class Bithumb(string accessKey, string secretKey) : Market(accessKey, secretKey), IMarket
{
    public JwtPayload GenerateJwtPayload()
    {
        var payload = new JwtPayload
        {
            { "access_key", AccessKey },
            { "nonce", Guid.NewGuid().ToString() },
            { "timestamp", DateTimeOffset.Now.ToUnixTimeMilliseconds()}
        };

        return payload;
    }
    
    public JwtPayload GenerateJwtPayload(Dictionary<string, string> parameters)
    {
        var payload = new JwtPayload
        {
            { "access_key", AccessKey },
            { "nonce", Guid.NewGuid().ToString() },
            { "timestamp", DateTimeOffset.Now.ToUnixTimeMilliseconds()},
            { "query_hash", JwtHelper.GenerateQuery(parameters) },
            { "query_hash_alg", "SHA512" }
        };

        return payload;
    }

    public async Task<string[]?> RequestMarketCodes()
    {
        return await RequestGet<string[], MarketCodesResponse>(
            "https://api.bithumb.com/v1/market/all?isDetails=false");
    }

    public async Task<double> RequestTicker(string marketCode)
    {
        return await RequestGet<double, TickerResponse>(
            $"https://api.bithumb.com/v1/ticker?markets={marketCode}");
    }

    public async Task<MarketOrderBook?> RequestMarketOrderbook(string marketCode)
    {
        return await RequestGet<MarketOrderBook, MarketOrderBookResponse>(
            $"https://api.bithumb.com/v1/orderbook?markets={marketCode}");
    }
    
    public async Task<double> RequestBalance(string coinSymbol)
    {
        var payload = GenerateJwtPayload();
        var dicBalance = await RequestJwtGet<Dictionary<string, double>, MarketBalanceResponse>("https://api.bithumb.com/v1/accounts", payload);

        if (dicBalance == null)
            return 0;
        
        return dicBalance.GetValueOrDefault(coinSymbol);
    }
    
    public async Task<bool> RequestCheckOrder(string uuid)
    {
        var payload = GenerateJwtPayload(new Dictionary<string, string>{{"uuid", uuid}});
        return await RequestJwtGet<bool, MarketCheckOrderResponse>($"https://api.bithumb.com/v1/order?uuid={uuid}", payload);
    }
    
    public async Task<string?> RequestBuy(string marketCode, double volume, double price)
    {
        var order = new Dictionary<string, string>
        {
            { "market", marketCode },
            { "side", "bid" },
            { "volume", volume.ToString(CultureInfo.InvariantCulture) },
            { "price", price.ToString(CultureInfo.InvariantCulture) },
            { "ord_type", "limit" }
        };
        
        var payload = GenerateJwtPayload(order);
        return await RequestJwtPost<string, MarketBuyResponse>($"https://api.bithumb.com/v1/orders", payload, order);
    }
    
    public async Task<bool> RequestCancelOrder(string uuid)
    {
        var payload = GenerateJwtPayload(new Dictionary<string, string>{{"uuid", uuid}});
        return await RequestJwtDelete<bool, MarketCancelOrderResponse>($"https://api.bithumb.com/v1/order?uuid={uuid}", payload);
    }

    public async Task<string?> RequestSell(string marketCode, double volume, double price)
    {
        var order = new Dictionary<string, string>
        {
            { "market", marketCode },
            { "side", "ask" },
            { "volume", volume.ToString(CultureInfo.InvariantCulture) },
            { "price", price.ToString(CultureInfo.InvariantCulture) },
            { "ord_type", "limit" }
        };
        
        var payload = GenerateJwtPayload(order);
        return await RequestJwtPost<string, MarketSellResponse>($"https://api.bithumb.com/v1/orders", payload, order);
    }
}