using HttpService;
using SharedClass;

namespace CoinAutoTradeService;

public class DeleteAllCoinTradeDataProtocol(CoinAutoTradeServer server) : HttpServiceProtocol<HttpServiceServer, DeleteAllCoinTradeDataRequest, DeleteAllCoinTradeDataResponse>(server)
{
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    protected override async Task<(EResponseCode, DeleteAllCoinTradeDataResponse?)> MakeResponseDataAsync(string id, DeleteAllCoinTradeDataRequest request)
    {
        var result = CoinTradeDataManager.DeleteAllCoinTradeData(id);
        if (!result)
            return (EResponseCode.DeleteAllCoinTradeDataFailed, null);
            
        if (!server.DicProcess.TryGetValue(id, out var value))
            return (EResponseCode.Success, new DeleteAllCoinTradeDataResponse());
        
        var (client, _) = value;
        result = await client.InnerRequestStartAllCoinAutoTradeAsync([]);
        return !result ? (EResponseCode.DeleteAllCoinTradeDataFailed, null) : (EResponseCode.Success, new DeleteAllCoinTradeDataResponse());
    }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
}