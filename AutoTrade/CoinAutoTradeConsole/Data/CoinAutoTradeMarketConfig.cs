using System.Text;
using SharedClass;

namespace CoinAutoTradeConsole;

public class CoinAutoTradeMarketConfig(string ip, string userId, EMarketType marketType, string marketApiKey, string marketSecretKey, string telegramApiToken, long telegramChatId)
{
    public string Ip { get; } = ip;
    public string UserId { get; } = userId;
    public EMarketType MarketType { get; } = marketType;
    public string MarketApiKey { get; } = marketApiKey;
    public string MarketSecretKey { get; } = marketSecretKey;
    public string TelegramApiToken { get; } = telegramApiToken;
    public long TelegramChatId { get; } = telegramChatId;

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine("IP: " + Ip);
        sb.AppendLine("UserId: " + UserId);
        sb.AppendLine("MarketType: " + MarketType);
        sb.AppendLine("MarketApiKey: " + MarketApiKey);
        sb.AppendLine("MarketSecretKey: " + MarketSecretKey);
        sb.AppendLine("TelegramApiToken: " + TelegramApiToken);
        sb.AppendLine("TelegramChatId: " + TelegramChatId);
        return sb.ToString();
    }
}