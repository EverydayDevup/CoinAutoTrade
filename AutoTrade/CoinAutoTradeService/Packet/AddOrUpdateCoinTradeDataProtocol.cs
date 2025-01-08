using HttpService;
using SharedClass;

namespace CoinAutoTradeService;

public class AddOrUpdateCoinTradeDataProtocol(CoinAutoTradeServer server) : HttpServiceProtocol<HttpServiceServer, AddOrUpdateCoinTradeDataRequest, AddOrUpdateCoinTradeDataResponse>(server)
{
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    protected override async Task<(EResponseCode, AddOrUpdateCoinTradeDataResponse?)> MakeResponseDataAsync(string id, AddOrUpdateCoinTradeDataRequest request)
    {
        var result = CoinTradeDataManager.AddOrUpdateCoinTradeData(id, request.CoinTradeData);
        if (!result)
            return (EResponseCode.AddCoinTradeDataFailed, null);

        if (!server.TryGetTradeClient(id, out var tradeClient))
            return (EResponseCode.Success, new AddOrUpdateCoinTradeDataResponse());
        
        result = await tradeClient!.InnerRequestStartAllCoinAutoTradeAsync(CoinTradeDataManager.GetAllCoinTradeData(id));
        return !result ? (EResponseCode.AddCoinTradeDataFailed, null) : (EResponseCode.Success, new AddOrUpdateCoinTradeDataResponse());
    }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
}