namespace CoinAutoTradeClient;

public static partial class CoinAutoTradeConsole
{
    private static LoggerService.LoggerService LoggerService { get; set; } = new();
    private static CoinAutoTradeMarketConfig? CoinAutoTradeMarketConfig { get; set; } = new();

    public static async Task Main()
    {
        // 마켓 정보를 가져옴
        CoinAutoTradeMarketConfig = await SelectCoinAutoTradeMarket();
        if (CoinAutoTradeMarketConfig == null)
        {
            LoggerService.ConsoleLog($"{nameof(SelectCoinAutoTradeMarket)} failed.");
            return;
        }

        var client = new CoinAutoTradeClient();
        await client.StartCoinAutoTradeAsync();
        Console.ReadLine();
    }
}