namespace CoinAutoTradeClient;

public static partial class CoinAutoTradeConsole
{
    private enum ECoinAutoTradeMode
    {
        None,
        SelectMenu,
        UserMarketInfo,
        WaitForExit,
        Exit
    }

    private static ECoinAutoTradeMode Mode { get; set; } = ECoinAutoTradeMode.None;
    private static Dictionary<ECoinAutoTradeMode, Func<Task<bool>>> _dicProcess = new();
    private static readonly ManualResetEventSlim ResetEvent = new ManualResetEventSlim(false);
    
    private static async Task ProcessAsync()
    {
        AppDomain.CurrentDomain.ProcessExit += (_, __) =>
        {
            LoggerService.ConsoleLog("Process exited");
            
            Task.Run(async () =>
            {
                if (Mode != ECoinAutoTradeMode.WaitForExit && Mode != ECoinAutoTradeMode.Exit)
                {
                    if (CoinAutoTradeClient != null)
                        await CoinAutoTradeClient.RequestAliveAsync();
                }
                
                ResetEvent.Set(); // 작업 완료 신호 전송
            });

            ResetEvent.Wait(); // 작업 완료될 때까지 대기
        };
        
        _dicProcess.Clear();
        _dicProcess.Add(ECoinAutoTradeMode.SelectMenu, ProcessSelectMenuAsync);
        _dicProcess.Add(ECoinAutoTradeMode.UserMarketInfo, ProcessUserMarketInfoAsync);
        
        Mode = ECoinAutoTradeMode.SelectMenu;
        while (Mode != ECoinAutoTradeMode.Exit)
        {
            if (CoinAutoTradeClient == null)
                continue;

            if (_dicProcess.TryGetValue(Mode, out var modeFunc))
            {
                LoggerService.ConsoleLog($"Process {nameof(Mode)} : {Mode}");
                var result = await modeFunc.Invoke();
                if (!result)
                    LoggerService.ConsoleLog($"{nameof(Mode)} : {Mode} Failed");
            }
            
            await Task.Delay(10);
        }
    }

    private static async Task<bool> ProcessSelectMenuAsync()
    {
        if (CoinAutoTradeClient == null)
            return false;
        
        var modes = new List<ECoinAutoTradeMode>()
        {
            ECoinAutoTradeMode.SelectMenu,
            ECoinAutoTradeMode.UserMarketInfo,
        };

        Mode = SelectMenu("Select Mode", modes);
        return true;
    }

    private static async Task<bool> ProcessUserMarketInfoAsync()
    {
        Mode = ECoinAutoTradeMode.SelectMenu;
        
        if (CoinAutoTradeClient == null)
            return false;
        
        var userMarketInfoRes = await CoinAutoTradeClient.RequestUserMarketInfoAsync();
        if (userMarketInfoRes == null || userMarketInfoRes.CoinTradeDataList == null) 
            return false;
        
        if (userMarketInfoRes.CoinTradeDataList.Count <= 0)
            LoggerService.ConsoleLog($"{nameof(userMarketInfoRes.CoinTradeDataList)} is empty");
        else
        {
            var coinTradeDataList = userMarketInfoRes.CoinTradeDataList;
            if (coinTradeDataList != null)
            {
                for (var i = 0; i < coinTradeDataList.Count; i++)
                {
                    LoggerService.ConsoleLog($"====== {i} ========");
                    LoggerService.ConsoleLog(coinTradeDataList[i].ToLog());
                    LoggerService.ConsoleLog($"===================");
                }
            }
        }
        
        return true;
    }
}