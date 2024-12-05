using HttpService;
using SharedClass;

namespace CoinAutoTrade.Packet;

public class UserMarketInfo(CoinAutoTradeServer server) : HttpServiceProtocol<HttpServiceServer, UserMarketInfoRequest>(server)
{
    protected override Tuple<int, ResponseBody?> MakeResponse(UserMarketInfoRequest request)
    {
        return new Tuple<int, ResponseBody?>(-1, null);
    }
}