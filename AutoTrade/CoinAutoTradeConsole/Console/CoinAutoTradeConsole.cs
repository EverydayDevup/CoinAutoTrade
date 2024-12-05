using CoinAutoTrade;

namespace CoinAutoTradeClient;

public static partial class CoinAutoTradeConsole
{
    private static LoggerService.LoggerService? LoggerService { get; set; }
    private static CoinAutoTradeMarketConfig? CoinAutoTradeMarketConfig { get; set; }
    private static CoinAutoTradeClient? CoinAutoTradeClient { get; set; }

    public static async Task Main()
    {
        LoggerService = new LoggerService.LoggerService();
        // 마켓 정보를 가져옴
        CoinAutoTradeMarketConfig = await SelectCoinAutoTradeMarket();
        if (CoinAutoTradeMarketConfig == null)
        {
            LoggerService.ConsoleLog($"{nameof(SelectCoinAutoTradeMarket)} failed.");
            return;
        }

        LoggerService = new LoggerService.LoggerService(CoinAutoTradeMarketConfig.TelegramApiToken,
            CoinAutoTradeMarketConfig.TelegramChatId);
        
        LoggerService.ConsoleLog($"Connect {nameof(CoinAutoTradeClient)} {CoinAutoTradeMarketConfig.MarketType}");
        
        CoinAutoTradeClient = new CoinAutoTradeClient(CoinAutoTradeMarketConfig.MarketType, CoinAutoTradeMarketConfig.UserId, 
            CoinAutoTradeMarketConfig.IP, CoinAutoTradeService.Port, 
            CoinAutoTradeMarketConfig.TelegramApiToken, CoinAutoTradeMarketConfig.TelegramChatId);

        var login = await CoinAutoTradeClient.RequestLoginAsync();
        if (!login)
            LoggerService.ConsoleLog($"{nameof(CoinAutoTradeClient.RequestLoginAsync)} error.");

        var alive = await CoinAutoTradeClient.RequestAliveAsync();
        if (!alive)
            LoggerService.ConsoleLog($"{nameof(CoinAutoTradeClient)} error.");

        Console.ReadLine();
    }
}