using System.Text;
using SharedClass;

namespace CoinAutoTradeConsole;

public class CoinAutoTradeMarketConfig(string ip, string userId, EMarketType marketType, string marketApiKey, string marketSecretKey, string telegramApiToken, long telegramChatId)
{
    public string Ip { get; } = ip; // proxy 서버의 주소
    public string UserId { get; } = userId; // 사용자의 아이디
    public EMarketType MarketType { get; } = marketType; // 거래소
    public string MarketApiKey { get; } = marketApiKey; // 거래소 api key
    public string MarketSecretKey { get; } = marketSecretKey; // 거래소 secret key
    public string TelegramApiToken { get; } = telegramApiToken; // 텔레그램 api key
    public long TelegramChatId { get; } = telegramChatId; // 텔레그램 chat id

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