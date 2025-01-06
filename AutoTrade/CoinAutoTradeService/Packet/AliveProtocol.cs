using HttpService;
using SharedClass;

namespace CoinAutoTradeService;

public class AliveProtocol(CoinAutoTradeServer server) : HttpServiceProtocol<HttpServiceServer, AliveRequest, AliveResponse>(server)
{
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    protected override async Task<(EResponseCode, AliveResponse?)> MakeResponseDataAsync(string id, AliveRequest request)
    {
        return (EResponseCode.Success, new AliveResponse());
    }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
}