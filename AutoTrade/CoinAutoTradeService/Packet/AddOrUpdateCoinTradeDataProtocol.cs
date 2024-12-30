using HttpService;
using SharedClass;

namespace CoinAutoTradeService;

public class AddOrUpdateCoinTradeDataProtocol(CoinAutoTradeServer server) : HttpServiceProtocol<HttpServiceServer, CoinTradeDataRequest, ResponseBody>(server)
{
    protected override Tuple<int, ResponseBody?> MakeResponse(string id, CoinTradeDataRequest request)
    {
        var result = CoinTradeDataManager.AddCoinTradeData(id, request.CoinTradeData);
        if (!result)
            return new Tuple<int, ResponseBody?>((int)EResponseCode.AddCoinTradeDataFailed, null);
            
        return new Tuple<int, ResponseBody?>((int)EResponseCode.Success, new ResponseBody());
    }
}