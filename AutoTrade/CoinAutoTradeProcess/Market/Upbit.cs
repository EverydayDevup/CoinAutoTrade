using System.Globalization;
using System.IdentityModel.Tokens.Jwt;

namespace CoinAutoTradeProcess;

public class Upbit(string accessKey, string secretKey) : Market(accessKey, secretKey), IMarket
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
            { "query_hash", JwtHelper.GenerateQuery(parameters) },
            { "query_hash_alg", "SHA512" }
        };

        return payload;
    }

    public async Task<MarketCodesResponse?> RequestMarketCodes()
    {
        return await RequestGet<MarketCodesResponse>(
            "https://api.upbit.com/v1/market/all?is_details=false");
    }

    public async Task<MarketTickerResponse?> RequestTicker(string marketCode)
    {
        return await RequestGet<MarketTickerResponse>(
            $"https://api.upbit.com/v1/ticker?markets={marketCode}");
    }

    public async Task<MarketOrderBookResponse?> RequestMarketOrderBook(string marketCode)
    {
        return await RequestGet<MarketOrderBookResponse>(
            $"https://api.upbit.com/v1/orderbook?level=0&markets={marketCode}");
    }

    public async Task<MarketBalanceResponse?> RequestBalance(string symbol)
    {
        var payload = GenerateJwtPayload();
        return await RequestJwtGet<MarketBalanceResponse>("https://api.upbit.com/v1/accounts", payload);
    }

    public async Task<MarketOrderResponse?> RequestOrder(string uuid)
    {
        var payload = GenerateJwtPayload(new Dictionary<string, string>{{"uuid", uuid}});
        return await RequestJwtGet<MarketOrderResponse>($"https://api.upbit.com/v1/order?uuid={uuid}", payload);
    }

    public async Task<MarketOrderResponse?> RequestBuy(string marketCode, double volume, double price)
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
        return await RequestJwtPost<MarketOrderResponse>($"https://api.upbit.com/v1/orders", payload, order);
    }

    public async Task<MarketOrderResponse?> RequestSell(string marketCode, double volume, double price)
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
        return await RequestJwtPost<MarketOrderResponse>($"https://api.upbit.com/v1/orders", payload, order);
    }

    public async Task<MarketOrderResponse?> RequestCancelOrder(string uuid)
    {
        var payload = GenerateJwtPayload(new Dictionary<string, string>{{"uuid", uuid}});
        return await RequestJwtDelete<MarketOrderResponse>($"https://api.upbit.com/v1/order?uuid={uuid}", payload);
    }
}