using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json;
using RestSharp;

namespace CoinAutoTradeProcess;

/// <summary>
/// 거래소와 관련된 공통 로직을 처리하는 클래스
/// </summary>
public abstract class Market(string accessKey, string secretKey)
{
    protected string AccessKey { get; private set; } = accessKey;
    private string SecretKey { get; set; } = secretKey;

    private string GenerateAuthToken(JwtPayload payload)
    {
        return JwtHelper.GenerateToken(payload, SecretKey);
    }
    
    /// <summary>
    /// public api로 get method를 사용하여 정보를 가져올 경우
    /// </summary>
    /// <param name="url"> 요청할 주소</param>
    /// <typeparam name="T"> 응답 결과</typeparam>
    /// <typeparam name="TK"> 응답을 처리할 클래스</typeparam>
    /// <returns></returns>
    protected static async Task<T?> RequestGet<T, TK>(string url) where TK : IResponse, new()
    {
        var options = new RestClientOptions(url);
        var client = new RestClient(options);
        var request = new RestRequest("");
        request.AddHeader("accept", "application/json");
        
        var res = await PacketHelper.RequestGetAsync<TK>(client, request);
        if (res?.IsSuccess == true)
            return (T)res.GetResult()!;
        
        Console.WriteLine($"{nameof(RequestGet)} failed");
        return default;
    }
    
    /// <summary>
    /// private api로 get method를 사용하여 정보를 가져올 경우
    /// </summary>
    /// <param name="url"> 요청할 주소</param>
    /// <param name="payload"> jwt 정보</param>
    /// <typeparam name="T"> 응답 결과</typeparam>
    /// <typeparam name="TK"> 응답을 처리할 클래스</typeparam>
    public async Task<T?> RequestJwtGet<T, TK>(string url, JwtPayload payload) where TK : IResponse, new()
    {
        var authToken = GenerateAuthToken(payload);
        
        var options = new RestClientOptions(url);
        var client = new RestClient(options);
        var request = new RestRequest("");
        request.AddHeader("accept", "application/json");
        request.AddHeader("Authorization", authToken);

        var res = await PacketHelper.RequestGetAsync<TK>(client, request);
        if (res?.IsSuccess == true)
            return (T)res.GetResult()!;
        
        Console.WriteLine($"{nameof(RequestJwtGet)} failed");
        return default;
    }
    
    /// <summary>
    /// private api로 get method를 사용하여 정보를 지울 때 사용
    /// </summary>
    /// <param name="url"> 요청할 주소</param>
    /// <param name="payload"> jwt 정보</param>
    /// <typeparam name="T"> 응답 결과</typeparam>
    /// <typeparam name="TK"> 응답을 처리할 클래스</typeparam>
    public async Task<T?> RequestJwtDelete<T, TK>(string url, JwtPayload payload) where TK : IResponse, new()
    {
        var authToken = GenerateAuthToken(payload);
        
        var options = new RestClientOptions(url);
        var client = new RestClient(options);
        var request = new RestRequest("");
        request.AddHeader("accept", "application/json");
        request.AddHeader("Authorization", authToken);

        var res = await PacketHelper.RequestDeleteAsync<TK>(client, request);
        if (res?.IsSuccess == true)
            return (T)res.GetResult()!;
        
        Console.WriteLine($"{nameof(RequestJwtDelete)} failed");
        return default;
    }
    
    /// <summary>
    /// private api로 get method를 사용하여 정보를 가져올 경우
    /// </summary>
    /// <param name="url"> 요청할 주소</param>
    /// <param name="payload"> jwt 정보</param>
    /// <param name="data"> post body 정보</param>
    /// <typeparam name="T"> 응답 결과</typeparam>
    /// <typeparam name="TK"> 응답을 처리할 클래스</typeparam>
    public async Task<T?> RequestJwtPost<T, TK>(string url, JwtPayload payload, Dictionary<string, string> data) where TK : IResponse, new()
    {
        var json = JsonConvert.SerializeObject(data);
        var authToken = GenerateAuthToken(payload);
        
        var options = new RestClientOptions(url);
        var client = new RestClient(options);
        var request = new RestRequest("");
        request.AddHeader("accept", "application/json");
        request.AddHeader("content-type", "application/json");
        request.AddHeader("Authorization", authToken);
        request.AddJsonBody(json, false);
        
        var res = await PacketHelper.RequestPostAsync<TK>(client, request);
        if (res?.IsSuccess == true)
            return (T)res.GetResult()!;
        
        Console.WriteLine($"{nameof(RequestJwtPost)} failed");
        return default;
    }
}