using System.Text.Json;
using HttpService;
using SharedClass;

namespace CoinAutoTrade.Packet;

public interface IHttpServiceProtocol
{
    public Tuple<int, string?> MakeResponse(string id, string? requestBody);
}

public class HttpServiceProtocol<T1, T2, T3>(T1 server) : IHttpServiceProtocol where T1 : HttpServiceServer where T2 : RequestBody where T3 : ResponseBody
{
    protected T1 Server { get; set; } = server;

    public Tuple<int, string?> MakeResponse(string id, string? requestBody)
    {
        if (string.IsNullOrEmpty(requestBody))
            return new Tuple<int, string?>(-1, null);
            
        var request = JsonSerializer.Deserialize<T2>(requestBody);
        if (request == null)
            return new Tuple<int, string?>(-1, null);

        var (code, body) = MakeResponse(id, request);

        return new Tuple<int, string?>(code, JsonSerializer.Serialize(body));
    }
    
    protected virtual Tuple<int, T3?> MakeResponse(string id, T2 request)
    {
        return new Tuple<int, T3?>(-1, null);
    }
}