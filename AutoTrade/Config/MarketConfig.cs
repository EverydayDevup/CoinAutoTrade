using System.Text;

namespace AutoTrade.Config;

public struct MarketConfig()
{
    public string Market { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    private StringBuilder LogStringBuilder { get; set; }

    public string ToLog()
    {
        LogStringBuilder ??= new StringBuilder();
        
        LogStringBuilder.Clear();
        LogStringBuilder.AppendLine("=============================");
        LogStringBuilder.AppendLine($"{nameof(Market)}: {Market}");
        LogStringBuilder.AppendLine($"{nameof(ApiKey)}: {ApiKey}");
        LogStringBuilder.AppendLine($"{nameof(SecretKey)}: {SecretKey}");
        LogStringBuilder.AppendLine("=============================");
        
        return LogStringBuilder.ToString();
    }
}