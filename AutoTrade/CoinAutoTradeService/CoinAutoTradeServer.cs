using HttpService;

namespace CoinAutoTrade;

public class CoinAutoTradeServer : HttpServiceServer
{
    public CoinAutoTradeServer(string ip, int port) : base(ip, port) { }
    public CoinAutoTradeServer(int port) : base(port) { }
    protected override string LogDirectoryPath => $"{nameof(CoinAutoTradeServer)}";
    
    private readonly CoinTradeDataManager _coinTradeDataManager = new();
    
    protected override Tuple<int, string> GenerateResponseData(int type, string data)
    {
        var code = ECoinAutoTradeResponseCode.NotFoundRequestType;
        var body = string.Empty;
        switch ((ECoinAutoTradeRequestType)type)
        {
            case ECoinAutoTradeRequestType.StartCoinAutoTrade:
            {
                if (!_coinTradeDataManager.Load(data, LoggerService))
                {
                    code = ECoinAutoTradeResponseCode.LoadCoinTradeDataListFail;
                    body = ECoinAutoTradeResponseCode.LoadCoinTradeDataListFail.ToString();
                }
                else
                {
                    code = ECoinAutoTradeResponseCode.Success;
                    body = data;
                }
            }
            break;
        }

        return new Tuple<int, string>((int)code, body);
    }
}