using HttpService;
using SharedClass;

namespace CoinAutoTradeService;

public class InnerAddOrUpdateCoinTradeDataProtocol(CoinAutoTradeServer server) : HttpServiceProtocol<HttpServiceServer, InnerAddOrUpdateCoinTradeDataRequest, InnerAddOrUpdateCoinTradeDataResponse>(server)
{
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    protected override async Task<(EResponseCode, InnerAddOrUpdateCoinTradeDataResponse?)> MakeResponseDataAsync(string id, InnerAddOrUpdateCoinTradeDataRequest request)
    {
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        
        var result = CoinTradeDataManager.AddOrUpdateCoinTradeData(id, request.CoinTradeData);
        if (!result)
            return (EResponseCode.InnerAddCoinTradeDataFailed, null);
        
        return (EResponseCode.Success, new InnerAddOrUpdateCoinTradeDataResponse());
    }
}