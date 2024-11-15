using System.Text;

namespace AutoTrade.Config;

public struct CoinConfig()
{
    public string MarketCode { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public double TotalAmount { get; set; }
    public double Amount { get; set; }
    public int BuyRate { get; set; }
    public int SellRate { get; set; }
    private StringBuilder LogStringBuilder { get; set; }

    public string ToLog()
    {
        LogStringBuilder ??= new StringBuilder();
        LogStringBuilder.Clear();
        LogStringBuilder.AppendLine("=============================");
        LogStringBuilder.AppendLine($"{nameof(MarketCode)}: {MarketCode}");
        LogStringBuilder.AppendLine($"{nameof(Currency)}: {Currency}");
        LogStringBuilder.AppendLine($"{nameof(TotalAmount)}: {TotalAmount}");
        LogStringBuilder.AppendLine($"{nameof(Amount)}: {Amount}");
        LogStringBuilder.AppendLine($"{nameof(BuyRate)}: {BuyRate}");
        LogStringBuilder.AppendLine($"{nameof(SellRate)}: {SellRate}");
        LogStringBuilder.AppendLine("=============================");
        
        return LogStringBuilder.ToString();
    }
}
