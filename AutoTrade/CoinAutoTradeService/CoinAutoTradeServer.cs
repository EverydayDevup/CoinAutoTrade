using CoinAutoTrade.Packet;
using HttpService;
using SharedClass;

namespace CoinAutoTrade;

public class CoinAutoTradeServer : HttpServiceServer
{
    public CoinAutoTradeServer(string ip, int port) : base(ip, port) { }
    public CoinAutoTradeServer(int port) : base(port) { }

    public CoinTradeDataManager CoinTradeDataManager { get; set; } = new();

    protected override void Init()
    {
        base.Init();
        DicHttpServiceProtocols.Add((int)EPacketType.UserMarketInfo, new UserMarketInfo(this));
    }

}