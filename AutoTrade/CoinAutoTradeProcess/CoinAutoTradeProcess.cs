namespace CoinAutoTradeProcess;

static class CoinAutoTradeProcess
{
    private static CoinAutoTradeProcessServer _coinAutoTradeProcessServer;
    
    static void Main(string[] args)
    {
        _coinAutoTradeProcessServer = new CoinAutoTradeProcessServer(55555);

        while (_coinAutoTradeProcessServer.IsRunning)
        {
            
        }
    }
}