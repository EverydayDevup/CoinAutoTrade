using HttpService;
using SharedClass;

namespace CoinAutoTrade.Packet;

public interface IHttpServiceProtocol
{
    public Tuple<int, ResponseBody?> MakeResponse(string? requestBody);
}

public abstract class HttpServiceProtocol<T>(T server) : IHttpServiceProtocol
    where T : HttpServiceServer
{
    protected T Server { get; set; } = server;

    public virtual Tuple<int, ResponseBody?> MakeResponse(string? requestBody)
    {
        return new Tuple<int, ResponseBody?>(-1, null);
    }
}