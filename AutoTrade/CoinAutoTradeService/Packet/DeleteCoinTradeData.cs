using HttpService;
using SharedClass;

namespace CoinAutoTrade.Packet;

public class DeleteCoinTradeData(CoinAutoTradeServer server) : HttpServiceProtocol<HttpServiceServer, CoinSymbolRequest, ResponseBody>(server)
{
    protected override Tuple<int, ResponseBody?> MakeResponse(string id, CoinSymbolRequest request)
    {
        var result = CoinTradeDataManager.DeleteCoinTradeData(id, request.Symbol);
        if (!result)
            return new Tuple<int, ResponseBody?>((int)EResponseCode.DeleteCoinTradeDataFailed, null);
            
        return new Tuple<int, ResponseBody?>((int)EResponseCode.Success, new ResponseBody());
    }
}