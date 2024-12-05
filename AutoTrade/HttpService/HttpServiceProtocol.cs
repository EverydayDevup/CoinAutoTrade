using System.Text.Json;
using HttpService;
using SharedClass;

namespace CoinAutoTrade.Packet;

public interface IHttpServiceProtocol
{
    public Tuple<int, ResponseBody?> MakeResponse(string? requestBody);
}

public class HttpServiceProtocol<T, TK>(T server) : IHttpServiceProtocol where T : HttpServiceServer where TK : RequestBody
{
    protected T Server { get; set; } = server;

    public Tuple<int, ResponseBody?> MakeResponse(string? requestBody)
    {
        if (string.IsNullOrEmpty(requestBody))
            return new Tuple<int, ResponseBody?>(-1, null);
            
        var request = JsonSerializer.Deserialize<TK>(requestBody);
        return request == null ? new Tuple<int, ResponseBody?>(-1, null) : MakeResponse(request);
    }
    
    protected virtual Tuple<int, ResponseBody?> MakeResponse(TK request)
    {
        return new Tuple<int, ResponseBody?>(-1, null);
    }
}