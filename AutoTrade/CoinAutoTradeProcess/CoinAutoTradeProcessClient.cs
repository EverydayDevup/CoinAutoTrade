using HttpService;

namespace CoinAutoTradeProcess;

public class CoinAutoTradeProcessClient : HttpServiceClient
{
    public CoinAutoTradeProcessClient(string ip, int port) : base(ip, port) { }

    public CoinAutoTradeProcessClient(int port) : base(port) { }
}