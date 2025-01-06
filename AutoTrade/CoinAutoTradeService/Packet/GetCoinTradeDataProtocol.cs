using HttpService;
using SharedClass;

namespace CoinAutoTradeService;

public class GetCoinTradeDataProtocol(CoinAutoTradeServer server) : HttpServiceProtocol<HttpServiceServer, GetCoinTradeDataRequest, GetCoinTradeDataResponse>(server)
{
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    protected override async Task<(EResponseCode, GetCoinTradeDataResponse?)> MakeResponseDataAsync(string id, GetCoinTradeDataRequest request)
    {
        var find = CoinTradeDataManager.GetCoinTradeData(id, request.Symbol);
        return (EResponseCode.Success, new GetCoinTradeDataResponse(find));
    }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
}