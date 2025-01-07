using HttpService;
using SharedClass;

namespace CoinAutoTradeService;

public class CoinAutoTradeClient(string id, string ip, int port, string? telegramApiToken, long telegramChatId)
    : HttpServiceClient(id, ip, port, telegramApiToken, telegramChatId)
{
    public async Task<bool> InnerRequestStartAllCoinAutoTradeAsync(List<CoinTradeData>? coinTradeDataList)
    {
        var res = await RequestAsync<InnerStartAllCoinAutoTradeRequest, InnerStartAllCoinAutoTradeResponse>
            (EPacketType.InnerStartAllCoinAutoTrade, new InnerStartAllCoinAutoTradeRequest(coinTradeDataList));
        
        return res != null;
    }
}