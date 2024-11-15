using System.Text;

namespace AutoTrade.Config;

public struct CoinConfig()
{
    public string MarketCode { get; set; } = string.Empty;
    private StringBuilder LogStringBuilder { get; set; }

    public string ToLog()
    {
        LogStringBuilder ??= new StringBuilder();
        LogStringBuilder.Clear();
        LogStringBuilder.AppendLine("=============================");
        LogStringBuilder.AppendLine($"{nameof(MarketCode)}: {MarketCode}");
        LogStringBuilder.AppendLine("=============================");
        
        return LogStringBuilder.ToString();
    }
}
