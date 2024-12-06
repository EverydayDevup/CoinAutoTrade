using HttpService;

namespace CoinAutoTradeProcess;

public class CoinAutoTradeProcessServer : HttpServiceServer
{
    public CoinAutoTradeProcessServer(string ip, int port) : base(ip, port) {}

    public CoinAutoTradeProcessServer(int port) : base(port) {}
    
    protected override void Init()
    {
        base.Init();
    }
}