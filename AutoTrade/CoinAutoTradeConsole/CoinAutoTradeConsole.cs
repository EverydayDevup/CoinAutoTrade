namespace CoinAutoTradeClient;

public static class CoinAutoTradeConsole
{
    public static async Task Main()
    {
        var client = new CoinAutoTradeClient();
        await client.StartCoinAutoTradeAsync();
        Console.ReadLine();
    }
}