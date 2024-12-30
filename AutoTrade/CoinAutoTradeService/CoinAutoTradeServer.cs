using System.Diagnostics;
using CoinAutoTrade.Packet;
using HttpService;
using SharedClass;

namespace CoinAutoTrade;

public class CoinAutoTradeServer(string ip, int port) : HttpServiceServer(ip, port)
{
    public Dictionary<string, Process> DicProcess { get; } = new();

    protected override void Init()
    {
        DicHttpServiceProtocols.Add((int)EPacketType.Login, new LoginProtocol(this));
        DicHttpServiceProtocols.Add((int)EPacketType.Alive, new AliveProtocol(this));
        DicHttpServiceProtocols.Add((int)EPacketType.GetAllCoinTradeData, new GetAllCoinTradeDataProtocol(this));
        DicHttpServiceProtocols.Add((int)EPacketType.DeleteAllCoinTradeData, new DeleteAllCoinTradeDataProtocol(this));
        DicHttpServiceProtocols.Add((int)EPacketType.AddOrUpdateCoinTradeData, new AddOrUpdateCoinTradeDataProtocol(this));
        DicHttpServiceProtocols.Add((int)EPacketType.GetCoinTradeData, new GetCoinTradeDataProtocol(this));
        DicHttpServiceProtocols.Add((int)EPacketType.DeleteCoinTradeData, new DeleteCoinTradeData(this));
        DicHttpServiceProtocols.Add((int)EPacketType.StartAllCoinAutoTrade, new StartAllCoinAutoTradeProtocol(this));
    }
}