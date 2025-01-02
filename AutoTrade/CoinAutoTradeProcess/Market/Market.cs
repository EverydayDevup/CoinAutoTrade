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
    /// <returns></returns>
    protected static async Task<T?> RequestGet<T>(string url) where T : IResponse, new()
    {
        var options = new RestClientOptions(url);
        var client = new RestClient(options);
        var request = new RestRequest("");
        request.AddHeader("accept", "application/json");
        
        return await PacketHelper.RequestGetAsync<T>(client, request);
    }
    
    /// <summary>
    /// private api로 get method를 사용하여 정보를 가져올 경우
    /// </summary>
    /// <param name="url"> 요청할 주소</param>
    /// <param name="payload"> jwt 정보</param>
    /// <typeparam name="T"> 응답 결과</typeparam>
    protected async Task<T?> RequestJwtGet<T>(string url, JwtPayload payload) where T : IResponse, new()
    {
        var authToken = GenerateAuthToken(payload);
        
        var options = new RestClientOptions(url);
        var client = new RestClient(options);
        var request = new RestRequest("");
        request.AddHeader("accept", "application/json");
        request.AddHeader("Authorization", authToken);

        return await PacketHelper.RequestGetAsync<T>(client, request);
    }
    
    /// <summary>
    /// private api로 get method를 사용하여 정보를 지울 때 사용
    /// </summary>
    /// <param name="url"> 요청할 주소</param>
    /// <param name="payload"> jwt 정보</param>
    /// <typeparam name="T"> 응답 결과</typeparam>
    protected async Task<T?> RequestJwtDelete<T>(string url, JwtPayload payload) where T : IResponse, new()
    {
        var authToken = GenerateAuthToken(payload);
        
        var options = new RestClientOptions(url);
        var client = new RestClient(options);
        var request = new RestRequest("");
        request.AddHeader("accept", "application/json");
        request.AddHeader("Authorization", authToken);

        return await PacketHelper.RequestDeleteAsync<T>(client, request);
    }
    
    /// <summary>
    /// private api로 get method를 사용하여 정보를 가져올 경우
    /// </summary>
    /// <param name="url"> 요청할 주소</param>
    /// <param name="payload"> jwt 정보</param>
    /// <param name="data"> post body 정보</param>
    /// <typeparam name="T"> 응답 결과</typeparam>
    protected async Task<T?> RequestJwtPost<T>(string url, JwtPayload payload, Dictionary<string, string> data) where T : IResponse, new()
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
        
        return await PacketHelper.RequestPostAsync<T>(client, request);
    }
}