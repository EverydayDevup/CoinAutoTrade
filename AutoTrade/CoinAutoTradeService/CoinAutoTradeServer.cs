using System.Diagnostics;
using HttpService;
using SharedClass;

namespace CoinAutoTradeService;

public class CoinAutoTradeServer(string ip, int port) : HttpServiceServer(ip, port)
{
    public Dictionary<string, (CoinAutoTradeClient, Process)> DicProcess { get; } = new();

    protected override void Init()
    {
        DicHttpServiceProtocols.Add(EPacketType.Alive, new AliveProtocol(this));
        DicHttpServiceProtocols.Add(EPacketType.Login, new LoginProtocol(this));
        DicHttpServiceProtocols.Add(EPacketType.GetAllCoinTradeData, new GetAllCoinTradeDataProtocol(this));
        DicHttpServiceProtocols.Add(EPacketType.DeleteAllCoinTradeData, new DeleteAllCoinTradeDataProtocol(this));
        DicHttpServiceProtocols.Add(EPacketType.AddOrUpdateCoinTradeData, new AddOrUpdateCoinTradeDataProtocol(this));
        DicHttpServiceProtocols.Add(EPacketType.GetCoinTradeData, new GetCoinTradeDataProtocol(this));
        DicHttpServiceProtocols.Add(EPacketType.DeleteCoinTradeData, new DeleteCoinTradeData(this));
        DicHttpServiceProtocols.Add(EPacketType.StartAllCoinAutoTrade, new StartAllCoinAutoTradeProtocol(this));
        DicHttpServiceProtocols.Add(EPacketType.StopAllCoinAutoTrade, new StopAllCoinAutoTradeProtocol(this));
        DicHttpServiceProtocols.Add(EPacketType.InnerAddOrUpdateCoinTradeData, new InnerAddOrUpdateCoinTradeDataProtocol(this));
    }

    public bool TryGetTradeClient(string id, out CoinAutoTradeClient? client)
    {
        client = null;
        
        if (!DicProcess.TryGetValue(id, out var value))
            return false;

        var (tradeClient, tradeServer) = value;
        if (tradeServer.HasExited)
            return false;
        
        client = tradeClient;
        return true;
    }
}