using HttpService;

namespace CoinAutoTrade;

public class CoinAutoTradeServer : HttpServiceServer
{
    public CoinAutoTradeServer(string ip, int port) : base(ip, port) { }
    public CoinAutoTradeServer(int port) : base(port) { }
    protected override string LogDirectoryPath => $"{nameof(CoinAutoTradeServer)}";

    protected override Tuple<int, string> GenerateResponseData(int type, string data)
    {
        switch ((ECoinAutoTradeRequestType)type)
        {
            case ECoinAutoTradeRequestType.StartCoinAutoTrade:
                return new Tuple<int, string>((int)ECoinAutoTradeResponseCode.Success, data);
            
            default:
                return new Tuple<int, string>((int)ECoinAutoTradeResponseCode.NotFoundRequestType, string.Empty);
        }
    }
}