using System.Text;
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

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine("IP: " + IP);
        sb.AppendLine("UserId: " + UserId);
        sb.AppendLine("MarketType: " + MarketType);
        sb.AppendLine("MarketApiKey: " + MarketApiKey);
        sb.AppendLine("MarketSecretKey: " + MarketSecretKey);
        sb.AppendLine("TelegramApiToken: " + TelegramApiToken);
        sb.AppendLine("TelegramChatId: " + TelegramChatId);
        return sb.ToString();
    }
}