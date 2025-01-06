using HttpService;
using SharedClass;

namespace CoinAutoTradeService;

public class AddOrUpdateCoinTradeDataProtocol(CoinAutoTradeServer server) : HttpServiceProtocol<HttpServiceServer, AddOrUpdateCoinTradeDataRequest, AddOrUpdateCoinTradeDataResponse>(server)
{
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    protected override async Task<(EResponseCode, AddOrUpdateCoinTradeDataResponse?)> MakeResponseDataAsync(string id, AddOrUpdateCoinTradeDataRequest request)
    {
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        
        var result = CoinTradeDataManager.AddOrUpdateCoinTradeData(id, request.CoinTradeData);
        if (!result)
            return (EResponseCode.AddCoinTradeDataFailed, null);
        
        return (EResponseCode.Success, new AddOrUpdateCoinTradeDataResponse());
    }
}