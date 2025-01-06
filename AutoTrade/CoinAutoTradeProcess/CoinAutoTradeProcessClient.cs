using HttpService;
using SharedClass;

namespace CoinAutoTradeProcess;

public class CoinAutoTradeProcessClient(EMarketType marketType, string id, string ip, int port, string telegramApiToken, long telegramChatId)
    : HttpServiceClient(id, ip, port, telegramApiToken, telegramChatId)
{
    public EMarketType MarketType { get; } = marketType;
    
    public async Task<InnerAddOrUpdateCoinTradeDataResponse?> RequestInnerAddOrUpdateCoinTradeDataAsync(string message, CoinTradeData coinTradeData)
    {
        var res = await RequestAsync<InnerAddOrUpdateCoinTradeDataRequest, InnerAddOrUpdateCoinTradeDataResponse>
            (EPacketType.InnerAddOrUpdateCoinTradeData, new InnerAddOrUpdateCoinTradeDataRequest(coinTradeData));

        if (res != null)
           await LoggerService.TelegramLogAsync($"[{MarketType}] [{message}] : {coinTradeData}");

        return res;
    }
}