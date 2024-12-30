using SharedClass;
using static System.Int32;

namespace CoinAutoTradeProcess;

public static class CoinAutoTradeProcess
{
    private static LoggerService.LoggerService LoggerService { get; } = new();
    private const string LocalHost = "127.0.0.1";
    
    public static void Main(string[] args)
    {
        if (!TryParse(args[0], out var port))
            return;
        
        if (!TryParse(args[1], out var market))
            return;

        var marketType = (EMarketType)market;
        var marketApiKey = args[2];
        var marketSecretKey = args[3];
        var telegramApiKey = args[4];
        
        if (!TryParse(args[5], out var telegramChatId))
            return;
        
        LoggerService.SetTelegramInfo($"{market}_{nameof(CoinAutoTradeProcessServer)}", telegramApiKey, telegramChatId);
        var coinAutoTradeProcessServer = new CoinAutoTradeProcessServer(marketType, marketApiKey, marketSecretKey, LocalHost, port);
    }
}