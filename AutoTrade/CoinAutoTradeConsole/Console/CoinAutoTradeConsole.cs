﻿using HttpService;

namespace CoinAutoTradeConsole;

// 콘솔 메뉴를 통해 자동 매매 관련 사용자의 입출력을 처리하는 클래스
public static partial class CoinAutoTradeConsole
{
    private static LoggerService.LoggerService LoggerService { get; } = new();
    private static CoinAutoTradeMarketConfig? CoinAutoTradeMarketConfig { get; set; }
    private static CoinAutoTradeClient? CoinAutoTradeClient { get; set; }

    public static async Task Main()
    {
        // 마켓 정보를 가져옴
        CoinAutoTradeMarketConfig = await SelectCoinAutoTradeMarket();
        if (CoinAutoTradeMarketConfig == null)
        {
            LoggerService.ConsoleLog($"{nameof(SelectCoinAutoTradeMarket)} failed.");
            return;
        }
        
        LoggerService.ConsoleLog($"Connect\n{nameof(CoinAutoTradeClient)} {CoinAutoTradeMarketConfig}");

        LoggerService.SetTelegramInfo(nameof(CoinAutoTradeConsole), CoinAutoTradeMarketConfig.TelegramApiToken,
            CoinAutoTradeMarketConfig.TelegramChatId);
        
        CoinAutoTradeClient = new CoinAutoTradeClient(CoinAutoTradeMarketConfig.MarketType, CoinAutoTradeMarketConfig.UserId, 
            CoinAutoTradeMarketConfig.Ip, HttpServiceUtil.CoinAutoTradeServicePort, 
            CoinAutoTradeMarketConfig.TelegramApiToken, CoinAutoTradeMarketConfig.TelegramChatId);

        var login = await CoinAutoTradeClient.RequestLoginAsync();
        if (login == null)
        {
            LoggerService.ConsoleLog($"{nameof(CoinAutoTradeClient.RequestLoginAsync)} error.");
            LoggerService.ConsoleLog("Please enter the key to end the process.");
            Console.ReadLine();
        }
        else
        {
            LoggerService.ConsoleLog($"{nameof(CoinAutoTradeClient.RequestLoginAsync)} success.");
            await ProcessAsync();
        }
    }
}