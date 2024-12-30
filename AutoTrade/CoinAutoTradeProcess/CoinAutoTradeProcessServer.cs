using HttpService;

namespace CoinAutoTradeProcess;

public class CoinAutoTradeProcessServer(string ip, int port) : HttpServiceServer(ip, port)
{
    protected override void Init()
    {
       
    }
}