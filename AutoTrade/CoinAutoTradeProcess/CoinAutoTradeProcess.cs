using System.Diagnostics;
using HttpService;
using SharedClass;

namespace CoinAutoTradeProcess;

/// <summary>
/// 마켓별 코인 매매를 관리
/// - 정해진 규칙에 따라 코인 매매를 자동으로 하는 기능
/// - 펌핑 코인을 체크해서, 코인 매매를 자동으로 하는 기능
/// </summary>
public static class CoinAutoTradeProcess
{
    private static LoggerService.LoggerService LoggerService { get; } = new();
    
    public static async Task Main(string[] args)
    {
        if (!int.TryParse(args[0], out var port))
            return;
        
        if (!int.TryParse(args[1], out var market))
            return;

        var marketType = (EMarketType)market;
        var marketApiKey = args[2];
        var marketSecretKey = args[3];
        var telegramApiKey = args[4];
        
        if (!int.TryParse(args[5], out var telegramChatId))
            return;
        
        if (!int.TryParse(args[6], out var parentProcessId))
            return;

        var id = args[7];
        var symmetricKey = args[8];
        
        ThreadPool.QueueUserWorkItem((state) =>
        {
            try
            {
                var parentProcess = Process.GetProcessById(parentProcessId);
                parentProcess.WaitForExit();
                Process.GetCurrentProcess().Kill();
            }
            catch
            {
                Process.GetCurrentProcess().Kill();
            }
        });
        
        LoggerService.SetTelegramInfo($"{(EMarketType)market}_{nameof(CoinAutoTradeProcessServer)}", telegramApiKey, telegramChatId);
        var coinAutoTradeProcessClient = new CoinAutoTradeProcessClient(marketType, id, HttpServiceUtil.LocalHost,
            HttpServiceUtil.CoinAutoTradeServicePort, telegramApiKey, telegramChatId);
        coinAutoTradeProcessClient.Key = symmetricKey;
        
        var coinAutoTradeProcessServer = new CoinAutoTradeProcessServer(coinAutoTradeProcessClient, marketType, marketApiKey, marketSecretKey, HttpServiceUtil.LocalHost, port);
        coinAutoTradeProcessServer.SetKey(id, symmetricKey);
        await coinAutoTradeProcessServer.HttpServiceServerRun();

        Console.ReadLine();
    }
}