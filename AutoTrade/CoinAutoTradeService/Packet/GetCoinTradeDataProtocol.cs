using HttpService;
using SharedClass;

namespace CoinAutoTrade.Packet;

public class GetCoinTradeDataProtocol(CoinAutoTradeServer server) : HttpServiceProtocol<HttpServiceServer, CoinSymbolRequest, CoinTradeDataResponse>(server)
{
    protected override Tuple<int, CoinTradeDataResponse?> MakeResponse(string id, CoinSymbolRequest request)
    {
        var res = new CoinTradeDataResponse();
        var result = CoinTradeDataManager.GetCoinTradeData(id, request.Symbol);
        res.CoinTradeData = result;
        return new Tuple<int, CoinTradeDataResponse?>((int)EResponseCode.Success, res);
    }
}