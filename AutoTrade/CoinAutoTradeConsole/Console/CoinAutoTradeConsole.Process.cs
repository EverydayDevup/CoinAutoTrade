using SharedClass;

namespace CoinAutoTradeClient;

public static partial class CoinAutoTradeConsole
{
    private enum ECoinAutoTradeMode
    {
        None,
        SelectMenu, // 메뉴 선택
        GetAllCoinTradeData, // 모든 코인 트레이드 정보 가져오기
        DeleteAllCoinTradeData, // 모든 코인 트레이드 정보 삭제하기
        AddOrUpdateCoinTradeData, // 특정 코인 트레이드 정보 추가하기
        GetCoinTradeData, // 특정 코인 트레이드 정보 가져오기
        DeleteCoinTradeData, // 특정 코인 트레이드 정보 삭제하기
        StartAllCoinAutoTrade, // 모든 코인 트레이드 시작
        StopAllCoinAutoTrade, // 모든 코인 트레이드 중단
        RestartAllCoinAutoTrade, // 모든 코인 트레이드 재시작
        StartCoinAutoTrade, // 특정 코인 트레이드 시작 
        StopCoinAutoTrade, // 특정 코인 트레이드 중단
        RestartCoinsAutoTrade, // 특정 코인 트레이드 재시작
        Exit, // 콘솔 종료
    }

    private static ECoinAutoTradeMode Mode { get; set; } = ECoinAutoTradeMode.None;

    private static List<ECoinAutoTradeMode>? _modes = null;

    private static List<ECoinAutoTradeMode> Modes
    {
        get
        {
            if (_modes == null)
            {
                _modes = new List<ECoinAutoTradeMode>();
                foreach (ECoinAutoTradeMode mode in Enum.GetValues(typeof(ECoinAutoTradeMode)))
                {
                    if (mode != ECoinAutoTradeMode.None)
                        _modes.Add(mode);
                }
            }

            return _modes;
        }
    }
    
    private static Dictionary<ECoinAutoTradeMode, Func<Task<bool>>> DicProcess { get; set; } = new();
    
    private static async Task ProcessAsync()
    {   
        DicProcess.Clear();
        DicProcess.Add(ECoinAutoTradeMode.SelectMenu, SelectMenuAsync);
        DicProcess.Add(ECoinAutoTradeMode.GetAllCoinTradeData, AllCoinTradeDataAsync);
        DicProcess.Add(ECoinAutoTradeMode.DeleteAllCoinTradeData, DeleteAllCoinTradeDataAsync);
        DicProcess.Add(ECoinAutoTradeMode.AddOrUpdateCoinTradeData, AddCoinTradeDataAsync);
        DicProcess.Add(ECoinAutoTradeMode.GetCoinTradeData, GetCoinTradeDataAsync);
        DicProcess.Add(ECoinAutoTradeMode.DeleteCoinTradeData, DeleteCoinTradeDataAsync);
        DicProcess.Add(ECoinAutoTradeMode.StartAllCoinAutoTrade, StartAllCoinAutoTradeAsync);
        
        Mode = ECoinAutoTradeMode.SelectMenu;
        while (Mode != ECoinAutoTradeMode.Exit)
        {
            if (CoinAutoTradeClient == null)
                continue;

            LoggerService.ConsoleLog($"====================== {Mode} =========================");
            var alive = await CoinAutoTradeClient.RequestAliveAsync();
            if (!alive)
            {
                LoggerService.ConsoleLog($"Process {nameof(alive)} : {alive}");
                return;
            }

            if (Mode == ECoinAutoTradeMode.Exit)
                return;

            if (DicProcess.TryGetValue(Mode, out var modeFunc))
            {
                var mode = Mode;
                LoggerService.ConsoleLog($"Process {nameof(Mode)} : {Mode}");
                var result = await modeFunc.Invoke();
                if (!result)
                    LoggerService.ConsoleLog($"{nameof(Mode)} : {Mode} Failed");

                if (mode != ECoinAutoTradeMode.SelectMenu)
                {
                    LoggerService.ConsoleLog("Please press enter to select the menu.");
                    Console.ReadLine();
                    
                    if (Mode != ECoinAutoTradeMode.Exit)
                        Mode = ECoinAutoTradeMode.SelectMenu;
                }
            }
        }
    }

    private static async Task<bool> SelectMenuAsync()
    {
        if (CoinAutoTradeClient == null)
            return false;
        
        Mode = SelectMenu("Select Mode", Modes);
        return true;
    }

    private static async Task<bool> AllCoinTradeDataAsync()
    {
        if (CoinAutoTradeClient == null)
            return false;
        
        var res = await CoinAutoTradeClient.RequestGetAllCoinTradeDataAsync();
        if (res == null || res.CoinTradeDataList == null) 
            return false;
        
        if (res.CoinTradeDataList.Count <= 0)
            LoggerService.ConsoleLog($"{nameof(res.CoinTradeDataList)} is empty");
        else
        {
            var coinTradeDataList = res.CoinTradeDataList;
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

    private static async Task<bool> DeleteAllCoinTradeDataAsync()
    {
        if (CoinAutoTradeClient == null)
            return false;
        
        var res = await CoinAutoTradeClient.RequestDeleteAllCoinTradeDataAsync();
        return res != null;
    }
    
    private static async Task<bool> AddCoinTradeDataAsync()
    {
        if (CoinAutoTradeClient == null)
            return false;

        var coinTradeData = new CoinTradeData();
        MakeCoinTradeData(coinTradeData);
        
        var res = await CoinAutoTradeClient.RequestAddCoinTradeDataAsync(coinTradeData);
        return res != null;
    }
    
    private static async Task<bool> GetCoinTradeDataAsync()
    {
        if (CoinAutoTradeClient == null)
            return false;

        var symbol = GetText("Input symbol");
        symbol = symbol.ToUpper();
        var res = await CoinAutoTradeClient.RequestGetCoinTradeDataAsync(symbol);
        
        if (res == null || res.CoinTradeData == null)
            LoggerService.ConsoleLog($"not found {nameof(symbol)} : {symbol}");
        else
            LoggerService.ConsoleLog($"{res.CoinTradeData.ToLog()}");

        return true;
    }
    
    private static async Task<bool> DeleteCoinTradeDataAsync()
    {
        if (CoinAutoTradeClient == null)
            return false;

        var symbol = GetText("Input symbol");
        symbol = symbol.ToUpper();
        var res = await CoinAutoTradeClient.RequestDeleteCoinTradeDataAsync(symbol);
        
        if (res == null || res.CoinTradeData == null)
            LoggerService.ConsoleLog($"not found {nameof(symbol)} : {symbol}");
        else
            LoggerService.ConsoleLog($"{res.CoinTradeData.ToLog()}");

        return true;
    }

    private static void MakeCoinTradeData(CoinTradeData coinTradeData)
    {
        var confirm = false;
        do
        {
            coinTradeData.Symbol = GetText($"Input coin symbol :");
            coinTradeData.Symbol = coinTradeData.Symbol.ToUpper();
            coinTradeData.InvestTotalAmount = GetLong($"Input invest total amount : ");
            coinTradeData.MaxLossRate = GetDouble($"Input max loss rate (%) [total amount * max loss rate : ");
            coinTradeData.InvestRoundAmount = GetLong($"Input invest round amount [must amount < total amount] : ");
            coinTradeData.InitBuyPrice = GetDouble($"Input init buy price [-1 is immediate] : ");
            coinTradeData.MaxSellPrice = GetDouble($"Input max sell price [-1 is infinity] : ");
            coinTradeData.RoundBuyRate = GetDouble($"Input round buy rate [buy price * (1 + round buy rate)] : ");
            coinTradeData.RoundSellRate = GetDouble($"Input round sell rate [buy price * (1 - round buy rate)] : ");
            LoggerService.ConsoleLog($"{nameof(coinTradeData)} : {coinTradeData.ToLog()}");

            var validMessage = coinTradeData.GetValidMessage();
            if (!string.IsNullOrEmpty(validMessage))
                LoggerService.ConsoleLog($"{nameof(validMessage)} : {validMessage}");
            else
            {
                var input = GetLong("Enter 1 to save :");
                if (input > 0)
                    confirm = true;
            }
            
        } while (!confirm);
    }
    
    private static async Task<bool> StartAllCoinAutoTradeAsync()
    {
        if (CoinAutoTradeClient == null)
            return false;

        var res = await CoinAutoTradeClient.RequestStartAllCoinTradeDataAsync();
        if (res == null)
            return false;

        return true;
    }
}