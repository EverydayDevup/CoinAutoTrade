﻿using Newtonsoft.Json;
using SharedClass;
using File = System.IO.File;

namespace CoinAutoTradeClient;

public static partial class CoinAutoTradeConsole
{
    private enum ELoadCoinAutoTradeConfigMenu : byte
    {
        Create,
        Load,
        Delete,
    }
    
    private static async Task<CoinAutoTradeMarketConfig?> SelectCoinAutoTradeMarket()
    {
        var marketList = new List<EMarketType>();
        foreach (EMarketType type in Enum.GetValues(typeof(EMarketType)))
            marketList.Add(type);
        var marketType = SelectMenu("Select market type : ", marketList);
        
        var userId = GetText("Input your user ID : ");
        
        var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), $"{nameof(CoinAutoTradeConsole)}", $"{nameof(CoinAutoTradeMarketConfig)}");

        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);
        
        var configUserKey = $"{marketType}_{userId}";
        var filePath = Path.Combine(directoryPath, $"{configUserKey}.json");
        
        List<ELoadCoinAutoTradeConfigMenu> menuList = new();
        
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
        {
            MarketType = marketType,
            UserId = userId,
            IP = GetText("Input your IP address: "),
            TelegramApiToken = GetText("Input your Telegram API token: "),
            TelegramChatId = GetNumber("Input your Telegram Chat ID: ")
        };

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