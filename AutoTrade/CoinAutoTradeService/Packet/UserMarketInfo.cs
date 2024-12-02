using HttpService;
using SharedClass;

namespace CoinAutoTrade.Packet;

public class UserMarketInfo(CoinAutoTradeServer server) : HttpServiceProtocol<HttpServiceServer>(server)
{
    public override Tuple<int, ResponseBody?> MakeResponse(string? requestBody)
    {
        return new Tuple<int, ResponseBody?>(-1, null);
    }
}