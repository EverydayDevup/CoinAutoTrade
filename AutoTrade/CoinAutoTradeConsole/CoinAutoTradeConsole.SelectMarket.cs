using Newtonsoft.Json;
using SharedClass;
using File = System.IO.File;

namespace CoinAutoTradeClient;

public partial class CoinAutoTradeConsole
{
    private enum ELoadCoinAutoTradeConfigMenu : byte
    {
        Create,
        Load,
        Delete,
    }
    
    private static CoinAutoTradeConfig? CoinAutoTradeConfig { get; set; } = new();
    private static async Task<bool> SelectCoinAutoTradeMarket()
    {
        MarketType = SelectMarketType();
        UserId = GetText("Input your user ID: ");
        CoinAutoTradeConfig = await GetUserDataAutoTradeConfigAsync();
        return CoinAutoTradeConfig != null;
    }
    
    private static EMarketType SelectMarketType()
    {
        var marketList = new List<EMarketType>();
        foreach (EMarketType type in Enum.GetValues(typeof(EMarketType)))
            marketList.Add(type);
        
        var marketMenu = SelectMenu("select market type : ", marketList);
        return marketList[marketMenu];
    }
    
    private static async Task<CoinAutoTradeConfig?> GetUserDataAutoTradeConfigAsync()
    {
        var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), $"{nameof(CoinAutoTradeConsole)}", "CoinAutoTradeConfig");

        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);

        var configUserKey = GetConfigUserKey(MarketType, UserId);
        var filePath = Path.Combine(directoryPath, $"{configUserKey}.json");
        
        List<ELoadCoinAutoTradeConfigMenu> menuList = new(); 

        if (File.Exists(filePath))
        {
            menuList.Add(ELoadCoinAutoTradeConfigMenu.Load);
            menuList.Add(ELoadCoinAutoTradeConfigMenu.Delete);
        }
        else
        {
            menuList.Add(ELoadCoinAutoTradeConfigMenu.Create);
        }
        
        var selectMenu = SelectMenu("select coin auto trade config menu : ", menuList);

        switch (menuList[selectMenu])
        {
            case ELoadCoinAutoTradeConfigMenu.Create:
                return await CreateUserDataAutoTradeConfigAsync(filePath);
            case ELoadCoinAutoTradeConfigMenu.Load:
                return await LoadUserDataAutoTradeConfigAsync(filePath);
            case ELoadCoinAutoTradeConfigMenu.Delete:
                File.Delete(filePath);
                return await GetUserDataAutoTradeConfigAsync();
            default:
                return null;
        }

        string GetConfigUserKey(EMarketType marketType, string userId) => $"{marketType}_{userId}";
    }

    private static async Task<CoinAutoTradeConfig?> CreateUserDataAutoTradeConfigAsync(string filePath)
    {
        var coinAutoTradeConfig = new CoinAutoTradeConfig();
        coinAutoTradeConfig.IP = GetText("Input your IP address: ");
        coinAutoTradeConfig.TelegramApiToken = GetText("Input your Telegram API token: ");

        long chatId = -1;
        do
        {
            var chatIdText = GetText("Input your Telegram Chat ID: ");
            if (long.TryParse(chatIdText, out chatId))
                coinAutoTradeConfig.TelegramChatId = chatId;
            
        } while (chatId < 0);

        var password = GetPassword();
        var encryptJson = Crypto.Encrypt(JsonConvert.SerializeObject(coinAutoTradeConfig), password);
        
        LoggerService.ConsoleLog("Create user data auto trade config");
        await File.WriteAllTextAsync(filePath, encryptJson);
        return coinAutoTradeConfig;
    }

    private static async Task<CoinAutoTradeConfig?> LoadUserDataAutoTradeConfigAsync(string filePath)
    {
        var password = GetPassword();
        var encryptJson = await File.ReadAllTextAsync(filePath);
        try
        {
            var plainJson = Crypto.Decrypt(encryptJson, password);
            LoggerService.ConsoleLog("Load user data auto trade config");
            return JsonConvert.DeserializeObject<CoinAutoTradeConfig>(plainJson);
        }
        catch (Exception ex)
        {
            LoggerService.ConsoleError($"{nameof(GetUserDataAutoTradeConfigAsync)} Failed to decrypt json: {ex.Message}");
            return null;
        }
    }
}