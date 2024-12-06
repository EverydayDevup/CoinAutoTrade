using static System.Int32;

namespace CoinAutoTradeProcess;

static class CoinAutoTradeProcess
{
    static void Main(string[] args)
    {
        TryParse(args[0], out var port);
        var coinAutoTradeProcessServer = new CoinAutoTradeProcessServer("127.0.0.1", port);
    }
}