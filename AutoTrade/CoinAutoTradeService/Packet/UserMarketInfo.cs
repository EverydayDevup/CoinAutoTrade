using HttpService;
using SharedClass;

namespace CoinAutoTrade.Packet;

public class UserMarketInfo(CoinAutoTradeServer server) : HttpServiceProtocol<HttpServiceServer, UserMarketInfoRequest, UserMarketInfoResponse>(server)
{
    protected override Tuple<int, UserMarketInfoResponse?> MakeResponse(string id, UserMarketInfoRequest request)
    {
        return new Tuple<int, UserMarketInfoResponse?>(-1, null);
    }
}