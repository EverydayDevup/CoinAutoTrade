using System.Globalization;
using System.IdentityModel.Tokens.Jwt;

namespace CoinAutoTradeProcess;

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

    public async Task<MarketCodesResponse?> RequestMarketCodes()
    {
        return await RequestGet<MarketCodesResponse>(
            "https://api.bithumb.com/v1/market/all?isDetails=false");
    }

    public async Task<MarketTickerResponse?> RequestTicker(string marketCode)
    {
        return await RequestGet<MarketTickerResponse>(
            $"https://api.bithumb.com/v1/ticker?markets={marketCode}");
    }

    public async Task<MarketOrderBookResponse?> RequestMarketOrderBook(string marketCode)
    {
        return await RequestGet<MarketOrderBookResponse>(
            $"https://api.bithumb.com/v1/orderbook?markets={marketCode}");
    }
    
    public async Task<MarketBalanceResponse?> RequestBalance(string symbol)
    {
        var payload = GenerateJwtPayload();
        return await RequestJwtGet<MarketBalanceResponse>("https://api.bithumb.com/v1/accounts", payload);
    }
    
    public async Task<MarketOrderResponse?> RequestOrder(string uuid)
    {
        var payload = GenerateJwtPayload(new Dictionary<string, string>{{"uuid", uuid}});
        return await RequestJwtGet<MarketOrderResponse>($"https://api.bithumb.com/v1/order?uuid={uuid}", payload);
    }
    
    public async Task<MarketBuyResponse?> RequestBuy(string marketCode, double volume, double price)
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
        return await RequestJwtPost<MarketBuyResponse>($"https://api.bithumb.com/v1/orders", payload, order);
    }
    
    public async Task<MarketSellResponse?> RequestSell(string marketCode, double volume, double price)
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
        return await RequestJwtPost<MarketSellResponse>($"https://api.bithumb.com/v1/orders", payload, order);
    }
    
    public async Task<MarketCancelResponse?> RequestCancelOrder(string uuid)
    {
        var payload = GenerateJwtPayload(new Dictionary<string, string>{{"uuid", uuid}});
        return await RequestJwtDelete<MarketCancelResponse>($"https://api.bithumb.com/v1/order?uuid={uuid}", payload);
    }
}