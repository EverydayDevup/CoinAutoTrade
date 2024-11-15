using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoTrade.Packet;
using AutoTrade.Packet.Bithumb;
using Microsoft.IdentityModel.Tokens;
using RestSharp;

namespace AutoTrade.Market;

public class Bithumb : IMarket
{
    public string ApiKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;

    public void SetKey(string apiKey, string secretKey)
    {
        ApiKey = apiKey;
        SecretKey = secretKey;
    }
    
    private string GetAuthToken()
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);


        var token = new JwtSecurityToken(signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    private static string GenerateSignature(string payload, string secretKey)
    {
        using (var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(secretKey)))
        {
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
            return Convert.ToBase64String(hash);
        }
    }
    
    private string Base64UrlEncode(string input)
    {
        return Base64UrlEncode(Encoding.UTF8.GetBytes(input));
    }

    private string Base64UrlEncode(byte[] input)
    {
        return Convert.ToBase64String(input)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

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

    public async Task<long> RequestBalance(string currency)
    {
        var jwtToken = GetAuthToken();
        
        var options = new RestClientOptions("https://api.bithumb.com/v1/accounts");
        var client = new RestClient(options);
        var request = new RestRequest("");
        request.AddHeader("accept", "application/json");
        request.AddHeader("Authorization", $"Bearer {jwtToken}");

        var res = await PacketHelper.RequestAsync<MarketAccountResponse>(client, request);
        if (res is { IsSuccess: true })
            return res.DicBalances.TryGetValue(currency, out var balance) ? balance : 0;
        
        Console.WriteLine($"{nameof(RequestBalance)} failed");
        return 0;
    }
}