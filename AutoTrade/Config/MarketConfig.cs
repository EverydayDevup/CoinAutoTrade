using System.Text;

namespace AutoTrade.Config;

/// <summary>
/// 거래소 관련 정보
/// </summary>
public class MarketConfig()
{
    /// <summary>
    /// 거래소에서 발급 받은 API Key
    /// </summary>
    public string AccessKey { get; set; } = string.Empty;
    /// <summary>
    /// 거래소에서 발급 받은 Secret Key
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;

    private StringBuilder LogStringBuilder { get; set; } = new();

    public string ToLog()
    {
        LogStringBuilder.Clear();
        LogStringBuilder.AppendLine("=============================");
        LogStringBuilder.AppendLine($"{nameof(AccessKey)}: {AccessKey}");
        LogStringBuilder.AppendLine($"{nameof(SecretKey)}: {SecretKey}");
        LogStringBuilder.AppendLine("=============================");
        
        return LogStringBuilder.ToString();
    }
}