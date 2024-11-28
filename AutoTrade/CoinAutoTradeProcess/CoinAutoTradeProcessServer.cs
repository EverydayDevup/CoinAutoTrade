using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using HttpService;

namespace CoinAutoTradeProcess;

public class CoinAutoTradeProcessServer : HttpServiceServer
{
    public CoinAutoTradeProcessServer(string ip, int port) : base(ip, port) {}

    public CoinAutoTradeProcessServer(int port) : base(port) {}

    protected override Tuple<int, string> GenerateResponseData(int type, string data)
    {
        switch (type)
        {
            case 1:
                return new Tuple<int, string>(0, JsonSerializer.Serialize(new { Data = "test"}));
                break;
        }

        return base.GenerateResponseData(type, data);
    }
}