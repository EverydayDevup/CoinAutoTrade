using System.IdentityModel.Tokens.Jwt;
using AutoTrade.Packet;
using AutoTrade.Packet.Bithumb;
using Newtonsoft.Json;
using RestSharp;

namespace AutoTrade.Market;

public class Bithumb : IMarket
{
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;

    public void SetKey(string accessKey, string secretKey)
    {
        AccessKey = accessKey;
        SecretKey = secretKey;
    }

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
    
    public JwtPayload GenerateJwtPayload(string json)
    {
        var payload = new JwtPayload
        {
            { "access_key", AccessKey },
            { "nonce", Guid.NewGuid().ToString() },
            { "timestamp", DateTimeOffset.Now.ToUnixTimeMilliseconds()},
            { "query_hash", JwtHelper.GenerateQuery(json)},
            { "query_hash_alg", "SHA512" }
        };

        return payload;
    }
    
    public string GenerateAuthToken(JwtPayload payload)
    {
        return JwtHelper.GenerateToken(payload, SecretKey);
    }

    public async Task<MarketOrderInfo?> RequestOrderbook(string marketCode)
    {
        var options = new RestClientOptions($"https://api.bithumb.com/v1/orderbook?markets={marketCode}");
        var client = new RestClient(options);
        var request = new RestRequest("");
        request.AddHeader("accept", "application/json");
        var res = await PacketHelper.RequestGetAsync<MarketOrderbookResponse>(client, request);
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
        
        var res = await PacketHelper.RequestGetAsync<MarketCodeTickerResponse>(client, request);
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

        var res = await PacketHelper.RequestGetAsync<MarketCodeResponse>(client, request);
        if (res is { IsSuccess: true })
            return res.MarketCodes;
        
        Console.WriteLine($"{nameof(RequestMarketCodes)} failed");
        return null;
    }

    public async Task<double> RequestBalance(string coinSymbol)
    {
        var payload = GenerateJwtPayload();
        var authToken = GenerateAuthToken(payload);
        
        var options = new RestClientOptions("https://api.bithumb.com/v1/accounts");
        var client = new RestClient(options);
        var request = new RestRequest("");
        request.AddHeader("accept", "application/json");
        request.AddHeader("Authorization", authToken);

        var res = await PacketHelper.RequestGetAsync<MarketAccountResponse>(client, request);
        if (res is { IsSuccess: true })
            return res.DicBalances.GetValueOrDefault(coinSymbol, 0);
        
        Console.WriteLine($"{nameof(RequestBalance)} failed");
        return 0;
    }
    
    public async Task<bool> RequestCheckOrder(string uuid)
    {
        var payload = GenerateJwtPayload(new Dictionary<string, string>{{"uuid", uuid}});
        var authToken = GenerateAuthToken(payload);
        
        var options = new RestClientOptions($"https://api.bithumb.com/v1/order?uuid={uuid}");
        var client = new RestClient(options);
        var request = new RestRequest("");
        request.AddHeader("accept", "application/json");
        request.AddHeader("Authorization", authToken);
        
        var res = await PacketHelper.RequestGetAsync<MarketCheckOrderResponse>(client, request);
        if (res is { IsSuccess: true })
            return res.IsExist;
        
        Console.WriteLine($"{nameof(RequestCheckOrder)} failed");
        return false;
    }
    
    public async Task<string> RequestBuy(string marketCode, double volume, double price)
    {
        var order = new MarketOrder(marketCode, "bid", volume, price, "limit");
        var json = JsonConvert.SerializeObject(order);
        var payload = GenerateJwtPayload(json);
        var authToken = GenerateAuthToken(payload);
        
        var options = new RestClientOptions("https://api.bithumb.com/v1/orders");
        var client = new RestClient(options);
        var request = new RestRequest("");
        request.AddHeader("accept", "application/json");
        request.AddHeader("content-type", "application/json");
        request.AddHeader("charset", "utf-8");
        request.AddHeader("Authorization", authToken);
        request.AddJsonBody(json, false);
        
        var res = await PacketHelper.RequestPostAsync<MarkBuyOrderResponse>(client, request);
        if (res is { IsSuccess: true })
            return res.Uuid;
        
        return string.Empty;
    }

    public async Task<string> RequestSell(string coinSymbol, double volume)
    {
        var authToken = GenerateAuthToken(null);
        
        var options = new RestClientOptions("https://api.bithumb.com/v1/accounts");
        var client = new RestClient(options);
        var request = new RestRequest("");
        request.AddHeader("accept", "application/json");
        request.AddHeader("Authorization", authToken);
        return string.Empty;
    }
    
    public async Task<bool> RequestCancelOrder(string uuid)
    {
        var payload = GenerateJwtPayload(new Dictionary<string, string>{{"uuid", uuid}});
        var authToken = GenerateAuthToken(payload);
        
        var options = new RestClientOptions("https://api.bithumb.com/v1/accounts");
        var client = new RestClient(options);
        var request = new RestRequest("");
        request.AddHeader("accept", "application/json");
        request.AddHeader("Authorization", authToken);
        return true;
    }
}