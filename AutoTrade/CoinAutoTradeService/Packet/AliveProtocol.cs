using HttpService;
using SharedClass;

namespace CoinAutoTrade.Packet;

public class AliveProtocol(CoinAutoTradeServer server) : HttpServiceProtocol<HttpServiceServer, RequestBody, ResponseBody>(server)
{
    protected override Tuple<int, ResponseBody?> MakeResponse(string id, RequestBody request)
    {
        return new Tuple<int, ResponseBody?>((int)EResponseCode.Success, new ResponseBody());
    }
}