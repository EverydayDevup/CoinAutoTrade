using HttpService;

namespace CoinAutoTradeProcess;

public class CoinAutoTradeProcessClient : HttpServiceClient
{
    public CoinAutoTradeProcessClient(string ip, int port, string telegramApiToken, long telegramChatId) 
        : base(ip, port, telegramApiToken, telegramChatId)
    {
    }

    public CoinAutoTradeProcessClient(int port, string telegramApiToken, long telegramChatId)
        : base(port, telegramApiToken, telegramChatId)
    {
    }
}