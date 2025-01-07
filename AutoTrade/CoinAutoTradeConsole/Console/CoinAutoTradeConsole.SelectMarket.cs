using Newtonsoft.Json;
using SharedClass;
using File = System.IO.File;

namespace CoinAutoTradeConsole;

public static partial class CoinAutoTradeConsole
{
    private enum ELoadCoinAutoTradeConfigMenu : byte
    {
        Create,
        Load,
        Delete,
    }
    
    /// <summary>
    /// 거래소 정보를 선택
    /// </summary>
    private static async Task<CoinAutoTradeMarketConfig?> SelectCoinAutoTradeMarket()
    {
        var marketList = new List<EMarketType>();
        foreach (EMarketType type in Enum.GetValues(typeof(EMarketType)))
            marketList.Add(type);
        
        var marketType = SelectMenu("Select market type : ", marketList);
        var userId = GetText("Input your user ID : ");
        var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), $"{nameof(CoinAutoTradeMarketConfig)}");

        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);

        userId = Crypto.GetSha256Hash(userId);
        var configUserKey = $"{marketType}_{userId}";
        var filePath = Path.Combine(directoryPath, $"{configUserKey}.json");
        
        List<ELoadCoinAutoTradeConfigMenu> menuList = [];
        
        if (File.Exists(filePath))
        {
            menuList.Add(ELoadCoinAutoTradeConfigMenu.Create);
            menuList.Add(ELoadCoinAutoTradeConfigMenu.Load);
            menuList.Add(ELoadCoinAutoTradeConfigMenu.Delete);
        }
        else
        {
            menuList.Add(ELoadCoinAutoTradeConfigMenu.Create);
        }
        
        var menu = SelectMenu("Select coin auto trade config menu : ", menuList);

        switch (menu)
        {
            case ELoadCoinAutoTradeConfigMenu.Create:
                return await CreateUserDataAutoTradeConfigAsync(marketType, userId, filePath);
            case ELoadCoinAutoTradeConfigMenu.Load:
                return await LoadUserDataAutoTradeConfigAsync(filePath);
            case ELoadCoinAutoTradeConfigMenu.Delete:
                File.Delete(filePath);
                return await SelectCoinAutoTradeMarket();
            default:
                return null;
        }
    }
    
    private static async Task<CoinAutoTradeMarketConfig?> CreateUserDataAutoTradeConfigAsync(EMarketType marketType, string userId, string filePath)
    {
        var coinAutoTradeConfig = new CoinAutoTradeMarketConfig
        (
            GetText("Input your IP address: "),
            userId,
            marketType,
            GetText("Input your API key: "),
            GetText("Input your Secret key: "),
            GetText("Input your Telegram API token: "),
           GetLong("Input your Telegram Chat ID: ")
        );

        var password = GetPassword();
        var encryptJson = Crypto.Encrypt(JsonConvert.SerializeObject(coinAutoTradeConfig), password);
        
        LoggerService.ConsoleLog("Create user data auto trade config");
        await File.WriteAllTextAsync(filePath, encryptJson);
        return coinAutoTradeConfig;
    }

    private static async Task<CoinAutoTradeMarketConfig?> LoadUserDataAutoTradeConfigAsync(string filePath)
    {
        var password = GetPassword();
        var encryptJson = await File.ReadAllTextAsync(filePath);
        try
        {
            var plainJson = Crypto.Decrypt(encryptJson, password);
            LoggerService.ConsoleLog("Load user data auto trade config");
            return JsonConvert.DeserializeObject<CoinAutoTradeMarketConfig>(plainJson);
        }
        catch (Exception ex)
        {
            LoggerService.ConsoleError($"{nameof(LoadUserDataAutoTradeConfigAsync)} Failed to decrypt json: {ex.Message}");
            return null;
        }
    }
}