using HttpService;
using SharedClass;

namespace CoinAutoTradeService;

public class StopAllCoinAutoTradeProtocol(CoinAutoTradeServer server): HttpServiceProtocol<HttpServiceServer, StopAllCoinTradeDataRequest, StopAllCoinTradeDataResponse>(server)
{
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    protected override async Task<(EResponseCode, StopAllCoinTradeDataResponse?)> MakeResponseDataAsync(string id, StopAllCoinTradeDataRequest request)
    {
        if (!server.DicProcess.TryGetValue(id, out var value))
            return (EResponseCode.Success, new StopAllCoinTradeDataResponse());
        
        var (_, tradeServer) = value;
        if (!tradeServer.HasExited)
            tradeServer.Kill();

        server.DicProcess.Remove(id);

        return (EResponseCode.Success, new StopAllCoinTradeDataResponse());
    }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
}