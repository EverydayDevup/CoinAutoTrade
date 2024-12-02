using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using HttpService;

namespace CoinAutoTradeProcess;

public class CoinAutoTradeProcessServer : HttpServiceServer
{
    public CoinAutoTradeProcessServer(string ip, int port) : base(ip, port) {}

    public CoinAutoTradeProcessServer(int port) : base(port) {}
}