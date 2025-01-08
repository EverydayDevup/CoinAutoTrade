using SharedClass;

namespace CoinAutoTradeConsole;

public static partial class CoinAutoTradeConsole
{
    private enum ECoinAutoTradeMode
    {
        None,
        SelectMenu, // 메뉴 선택
        GetAllCoinTradeData, // 모든 코인 트레이드 정보 가져오기
        DeleteAllCoinTradeData, // 모든 코인 트레이드 정보 삭제하기
        GetCoinTradeData, // 특정 코인 트레이드 정보 가져오기
        AddOrUpdateCoinTradeData, // 특정 코인 트레이드 정보 추가하기
        DeleteCoinTradeData, // 특정 코인 트레이드 정보 삭제하기
        StartAllCoinAutoTrade, // 모든 코인 트레이드 시작
        StopAllCoinAutoTrade, // 모든 코인 트레이드 중단
        Exit, // 콘솔 종료
    }

    private static ECoinAutoTradeMode Mode { get; set; } = ECoinAutoTradeMode.None;

    private static List<ECoinAutoTradeMode>? _modes;

    private static List<ECoinAutoTradeMode> Modes
    {
        get
        {
            if (_modes == null)
            {
                _modes = [];
                foreach (ECoinAutoTradeMode mode in Enum.GetValues(typeof(ECoinAutoTradeMode)))
                {
                    if (mode != ECoinAutoTradeMode.None)
                        _modes.Add(mode);
                }
            }

            return _modes;
        }
    }

    private static List<ECoinTradeState>? _states;

    private static List<ECoinTradeState> States
    {
        get
        {
            if (_states == null)
            {
                _states = [];
                foreach (ECoinTradeState state in Enum.GetValues(typeof(ECoinTradeState)))
                    _states.Add(state);
            }

            return _states;
        }
    }
    
    private static List<ECoinTradeType>? _tradeTypes;

    private static List<ECoinTradeType> TradeTypes
    {
        get
        {
            if (_tradeTypes == null)
            {
                _tradeTypes = [];
                foreach (ECoinTradeType type in Enum.GetValues(typeof(ECoinTradeType)))
                    _tradeTypes.Add(type);
            }

            return _tradeTypes;
        }
    }
    
    private static Dictionary<ECoinAutoTradeMode, Func<Task<bool>>> DicProcess { get; set; } = new();

    private static void Init()
    {
        DicProcess.Clear();
        DicProcess.Add(ECoinAutoTradeMode.SelectMenu, SelectMenuAsync);
        DicProcess.Add(ECoinAutoTradeMode.GetAllCoinTradeData, GetAllCoinTradeDataAsync);
        DicProcess.Add(ECoinAutoTradeMode.DeleteAllCoinTradeData, DeleteAllCoinTradeDataAsync);
        DicProcess.Add(ECoinAutoTradeMode.AddOrUpdateCoinTradeData, AddOrUpdateCoinTradeDataAsync);
        DicProcess.Add(ECoinAutoTradeMode.GetCoinTradeData, GetCoinTradeDataAsync);
        DicProcess.Add(ECoinAutoTradeMode.DeleteCoinTradeData, DeleteCoinTradeDataAsync);
        DicProcess.Add(ECoinAutoTradeMode.StartAllCoinAutoTrade, StartAllCoinAutoTradeAsync);
        DicProcess.Add(ECoinAutoTradeMode.StopAllCoinAutoTrade, StopAllCoinAutoTradeAsync);
    }
    
    private static async Task ProcessAsync()
    {
        Init();
        
        Mode = ECoinAutoTradeMode.SelectMenu;
        while (Mode != ECoinAutoTradeMode.Exit)
        {
            if (CoinAutoTradeClient == null)
                continue;

            LoggerService.ConsoleLog($"====================== {Mode} =========================");
            var alive = await CoinAutoTradeClient.RequestAliveAsync();
            if (alive == null)
            {
                LoggerService.ConsoleLog($"Server Process {nameof(alive)} : {alive != null}");
                return;
            }
            
            LoggerService.ConsoleLog($"Trade Server Process Exist = {alive.IsTradeProcess}");

            if (Mode == ECoinAutoTradeMode.Exit)
                return;

            if (!DicProcess.TryGetValue(Mode, out var modeFunc))
            {
                LoggerService.ConsoleLog($"Undefined {nameof(Mode)} : {Mode}");
                return;
            }
            
            var mode = Mode;
            LoggerService.ConsoleLog($"{nameof(Mode)} : {Mode}");
            
            var result = await modeFunc.Invoke();
            if (!result)
                LoggerService.ConsoleLog($"{nameof(Mode)} : {Mode} Failed");

            if (mode == ECoinAutoTradeMode.SelectMenu)
                continue;
            
            LoggerService.ConsoleLog("Please press enter to select the menu.");
            Console.ReadLine();
                    
            if (Mode != ECoinAutoTradeMode.Exit)
                Mode = ECoinAutoTradeMode.SelectMenu;
        }
    }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    private static async Task<bool> SelectMenuAsync()
    {
        if (CoinAutoTradeClient == null)
            return false;
        
        Mode = SelectMenu("Select Mode : ", Modes);
        return true;
    }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

    private static async Task<bool> GetAllCoinTradeDataAsync()
    {
        if (CoinAutoTradeClient == null)
            return false;
        
        var res = await CoinAutoTradeClient.RequestGetAllCoinTradeDataAsync();
        
        if (res?.CoinTradeDataList is not { Count: > 0 })
            LoggerService.ConsoleLog($"{nameof(res.CoinTradeDataList)} is empty");
        else
        {
            var coinTradeDataList = res.CoinTradeDataList;
            if (coinTradeDataList == null) 
                return true;
            
            for (var i = 0; i < coinTradeDataList.Count; i++)
            {
                LoggerService.ConsoleLog($"====== {i} ========");
                LoggerService.ConsoleLog($"{coinTradeDataList[i]}");
                LoggerService.ConsoleLog($"===================");
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
    
    private static async Task<bool> AddOrUpdateCoinTradeDataAsync()
    {
        if (CoinAutoTradeClient == null)
            return false;

        var coinTradeData = await MakeCoinTradeData();
        if (coinTradeData == null)
            return false;
        
        var res = await CoinAutoTradeClient.RequestAddOrUpdateCoinTradeDataAsync(coinTradeData);
        return res != null;
    }
    
    private static async Task<bool> GetCoinTradeDataAsync()
    {
        if (CoinAutoTradeClient == null)
            return false;

        var symbol = GetText("Input symbol");
        symbol = symbol.ToUpper();
        
        var res = await CoinAutoTradeClient.RequestGetCoinTradeDataAsync(symbol);

        LoggerService.ConsoleLog(res?.CoinTradeData == null
            ? $"not found {nameof(symbol)} : {symbol}"
            : $"{res.CoinTradeData}");

        return true;
    }
    
    private static async Task<bool> DeleteCoinTradeDataAsync()
    {
        if (CoinAutoTradeClient == null)
            return false;

        var symbol = GetText("Input symbol");
        symbol = symbol.ToUpper();
        
        var res = await CoinAutoTradeClient.RequestDeleteCoinTradeDataAsync(symbol);
        
        if (res != null)
            LoggerService.ConsoleLog($"Delete CoinTradeData : {symbol}");

        return true;
    }

    private static async Task<CoinTradeData?> MakeCoinTradeData()
    {
        if (CoinAutoTradeClient == null)
            return null;
        
        CoinTradeData? coinTradeData;
        var confirm = false;
        do
        {
            var symbol = GetText($"Input coin symbol:");
            
            var res = await CoinAutoTradeClient.RequestGetCoinTradeDataAsync(symbol);
            coinTradeData = res?.CoinTradeData ?? new CoinTradeData();

            coinTradeData.Symbol = symbol.ToUpper();
            coinTradeData.State = SelectMenu($"Input state ({coinTradeData.State}): ", States);
            coinTradeData.TradeType = SelectMenu($"Input trade type ({coinTradeData.TradeType}) : ", TradeTypes);
            coinTradeData.InvestRoundAmount = GetDouble($"Input invest round amount ({coinTradeData.InvestRoundAmount}): ");
            coinTradeData.InitBuyPrice = GetDouble($"Input init buy price [-1 is immediate] ({coinTradeData.InitBuyPrice}): ");
            coinTradeData.MaxSellPrice = GetDouble($"Input max sell price [-1 is infinity] ({coinTradeData.MaxSellPrice}): ");
            coinTradeData.RoundBuyRate = GetDouble($"Input round buy rate [buy price * (1 + round buy rate)] ({coinTradeData.RoundBuyRate}): ");
            coinTradeData.RoundSellRate = GetDouble($"Input round sell rate [buy price * (1 - round buy rate)] ({coinTradeData.RoundSellRate}): ");
            coinTradeData.RebalancingMaxCount = GetDouble($"Input rebanlancing max count ({coinTradeData.RebalancingMaxCount}): ");
            coinTradeData.RebalancingCount = GetDouble($"Input rebanlancing count ({coinTradeData.RebalancingCount}): ");
            coinTradeData.BuyPrice = GetDouble($"Input buy price ({coinTradeData.BuyPrice}): ");
            coinTradeData.BuyCount = GetDouble($"Input buy count ({coinTradeData.BuyCount}): ");
            coinTradeData.SellPrice = GetDouble($"Input sell price ({coinTradeData.SellPrice}): ");
            LoggerService.ConsoleLog($"{nameof(coinTradeData)} : {coinTradeData}");

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

        return coinTradeData;
    }
    
    private static async Task<bool> StartAllCoinAutoTradeAsync()
    {
        if (CoinAutoTradeClient == null || CoinAutoTradeMarketConfig == null)
            return false;

        var res = await CoinAutoTradeClient.RequestStartAllCoinTradeDataAsync
        (CoinAutoTradeMarketConfig.MarketType, CoinAutoTradeMarketConfig.MarketApiKey, CoinAutoTradeMarketConfig.MarketSecretKey,
            CoinAutoTradeMarketConfig.TelegramApiToken, CoinAutoTradeMarketConfig.TelegramChatId);
        
        return res != null;
    }
    
    private static async Task<bool> StopAllCoinAutoTradeAsync()
    {
        if (CoinAutoTradeClient == null || CoinAutoTradeMarketConfig == null)
            return false;

        var res = await CoinAutoTradeClient.RequestStopAllCoinTradeDataAsync();
        return res != null;
    }
}