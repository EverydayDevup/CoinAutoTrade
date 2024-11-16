using System.Text;

namespace AutoTrade.Config;

/// <summary>
/// 거래소 관련 정보
/// todo : 다음 버전에서 입력받아서 저장하는 방식을 고려
/// </summary>
public struct MarketConfig()
{
    /// <summary>
    /// 사용할 거래소의 이름
    /// </summary>
    public string Market { get; set; } = string.Empty;
    /// <summary>
    /// 거래소에서 발급 받은 API Key
    /// todo : 다음 버전에서 암호화 
    /// </summary>
    public string AccessKey { get; set; } = string.Empty;
    /// <summary>
    /// 거래소에서 발급 받은 Secret Key
    /// todo : 다음 버전에서 암호화
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;
    private StringBuilder LogStringBuilder { get; set; }

    public string ToLog()
    {
        LogStringBuilder ??= new StringBuilder();
        
        LogStringBuilder.Clear();
        LogStringBuilder.AppendLine("=============================");
        LogStringBuilder.AppendLine($"{nameof(Market)}: {Market}");
        LogStringBuilder.AppendLine($"{nameof(AccessKey)}: {AccessKey}");
        LogStringBuilder.AppendLine($"{nameof(SecretKey)}: {SecretKey}");
        LogStringBuilder.AppendLine("=============================");
        
        return LogStringBuilder.ToString();
    }
}