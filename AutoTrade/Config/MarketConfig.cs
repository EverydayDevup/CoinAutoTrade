using System.Text;

namespace AutoTrade.Config;

public struct MarketConfig()
{
    public string Market { get; set; } = string.Empty;
    private StringBuilder LogStringBuilder { get; set; }

    public string ToLog()
    {
        LogStringBuilder ??= new StringBuilder();
        
        LogStringBuilder.Clear();
        LogStringBuilder.AppendLine("=============================");
        LogStringBuilder.AppendLine($"{nameof(Market)}: {Market}");
        LogStringBuilder.AppendLine("=============================");
        
        return LogStringBuilder.ToString();
    }
}