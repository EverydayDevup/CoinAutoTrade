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
        
        if (!server.DicProcess.TryGetValue(id, out var value))
            return (EResponseCode.Success, new AddOrUpdateCoinTradeDataResponse());
        
        var (client, _) = value;
        result = await client.InnerRequestStartAllCoinAutoTradeAsync(CoinTradeDataManager.GetAllCoinTradeData(id));
        return !result ? (EResponseCode.DeleteAllCoinTradeDataFailed, null) : (EResponseCode.Success, new AddOrUpdateCoinTradeDataResponse());
    }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
}