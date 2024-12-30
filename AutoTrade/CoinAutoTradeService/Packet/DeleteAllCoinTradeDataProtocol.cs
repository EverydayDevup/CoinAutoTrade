using HttpService;
using SharedClass;

namespace CoinAutoTradeService;

public class DeleteAllCoinTradeDataProtocol(CoinAutoTradeServer server) : HttpServiceProtocol<HttpServiceServer, RequestBody, ResponseBody>(server)
{
    protected override Tuple<int, ResponseBody?> MakeResponse(string id, RequestBody request)
    {
        var result = CoinTradeDataManager.DeleteAllCoinTradeData(id);
        if (!result)
           return new Tuple<int, ResponseBody?>((int)EResponseCode.DeleteAllCoinTradeDataFailed, null);
            
        return new Tuple<int, ResponseBody?>((int)EResponseCode.Success, new ResponseBody());
    }
}