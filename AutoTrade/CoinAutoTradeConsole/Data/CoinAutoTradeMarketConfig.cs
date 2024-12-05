using SharedClass;

namespace CoinAutoTradeClient;

public class CoinAutoTradeMarketConfig
{
    public string IP { get; set; }
    public string UserId { get; set; }
    public EMarketType MarketType { get; set; }
    public string MarketApiKey { get; set; }
    public string MarketSecretKey { get; set; }
    public string TelegramApiToken { get; set; }
    public long TelegramChatId { get; set; }
}