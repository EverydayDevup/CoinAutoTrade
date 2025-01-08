using HttpService;
using SharedClass;

namespace CoinAutoTradeService;

/// <summary>
/// 내부의 거래소 매매 프로세스와 통신을 처리하기 위한 클라이언트
/// </summary>
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