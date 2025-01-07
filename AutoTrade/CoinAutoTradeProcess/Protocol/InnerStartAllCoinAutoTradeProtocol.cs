using HttpService;
using SharedClass;

namespace CoinAutoTradeProcess.Protocol;

public class InnerStartAllCoinAutoTradeProtocol(CoinAutoTradeProcessServer server) : HttpServiceProtocol<HttpServiceServer, InnerStartAllCoinAutoTradeRequest, InnerStartAllCoinAutoTradeResponse>(server)
{
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    protected override async Task<(EResponseCode, InnerStartAllCoinAutoTradeResponse?)> MakeResponseDataAsync(string id, InnerStartAllCoinAutoTradeRequest request)
    {
        server.CoinAutoTrade?.Reload(request.CoinTradeDataList);
        return (EResponseCode.Success, new InnerStartAllCoinAutoTradeResponse());
    }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
}