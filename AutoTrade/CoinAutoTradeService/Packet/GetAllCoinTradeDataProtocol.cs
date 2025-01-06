using HttpService;
using SharedClass;

namespace CoinAutoTradeService;

public class GetAllCoinTradeDataProtocol(CoinAutoTradeServer server)
    : HttpServiceProtocol<HttpServiceServer, GetAllCoinTradeDataRequest, GetAllCoinTradeDataResponse>(server)
{
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    protected override async Task<(EResponseCode, GetAllCoinTradeDataResponse?)> MakeResponseDataAsync(string id, GetAllCoinTradeDataRequest request)
    {
        return (EResponseCode.Success, new GetAllCoinTradeDataResponse(CoinTradeDataManager.GetAllCoinTradeData(id)));
    }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
}