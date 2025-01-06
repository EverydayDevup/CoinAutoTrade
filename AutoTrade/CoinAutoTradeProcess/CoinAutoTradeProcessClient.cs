using HttpService;

namespace CoinAutoTradeProcess;

public class CoinAutoTradeProcessClient(string id, string ip, int port, string telegramApiToken, long telegramChatId)
    : HttpServiceClient(id, ip, port, telegramApiToken, telegramChatId)
{
    
}