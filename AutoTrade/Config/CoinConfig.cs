using System.Text;

namespace AutoTrade.Config;

public struct CoinConfig()
{
    public string MarketCode { get; set; } = string.Empty;
    public double TotalAmount { get; set; }
    public double Amount { get; set; }
    public int BuyRate { get; set; }
    private StringBuilder LogStringBuilder { get; set; }

    public string ToLog()
    {
        LogStringBuilder ??= new StringBuilder();
        LogStringBuilder.Clear();
        LogStringBuilder.AppendLine("=============================");
        LogStringBuilder.AppendLine($"{nameof(MarketCode)}: {MarketCode}");
        LogStringBuilder.AppendLine($"{nameof(TotalAmount)}: {TotalAmount}");
        LogStringBuilder.AppendLine($"{nameof(Amount)}: {Amount}");
        LogStringBuilder.AppendLine("=============================");
        
        return LogStringBuilder.ToString();
    }
}
