namespace CoinAutoTrade;

public static class CoinAutoTradeService
{
    public static readonly int Port = 50000;
    
    public static void Main()
    {
        var server = new CoinAutoTradeServer(Port);
        while (server.IsRunning) { }
    }
}