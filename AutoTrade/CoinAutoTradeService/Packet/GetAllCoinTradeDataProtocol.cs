using HttpService;
using SharedClass;

namespace CoinAutoTradeService;

public class GetAllCoinTradeDataProtocol(CoinAutoTradeServer server) : HttpServiceProtocol<HttpServiceServer, RequestBody, GetAllCoinTradeDataResponse>(server)
{
    protected override Tuple<int, GetAllCoinTradeDataResponse?> MakeResponse(string id, RequestBody request)
    {
        var res = new GetAllCoinTradeDataResponse
        {
            CoinTradeDataList = CoinTradeDataManager.GetAllCoinTradeData(id)
        };

        return new Tuple<int, GetAllCoinTradeDataResponse?>((int)EResponseCode.Success, res);
    }
}