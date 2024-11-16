using System.Text;

namespace AutoTrade.Config;

public struct CoinConfig()
{
    /// <summary>
    /// 거래소에서 사용하는 마켓 코드 ex) KRW-BTC
    /// </summary>
    public string MarketCode { get; set; } = string.Empty;
    /// <summary>
    /// 코인 심볼
    /// </summary>
    public string Symbol { get; set; } = string.Empty;
    /// <summary>
    /// 전체 투자 금액
    /// </summary>
    public double TotalAmount { get; set; }
    /// <summary>
    /// 한번에 투자할 금액
    /// </summary>
    public double Amount { get; set; }
    /// <summary>
    /// 추가 매수 시 코인을 구매할 당시의 가격 * (1 + BuyRate) 로 다음 구매 가격을 결정함 
    /// </summary>
    public int BuyRate { get; set; }
    /// <summary>
    /// 손절 타이밍 계산 시 첫 구매 때는 총 투자 금액의 1%를 기준으로 하고, 이후 구매 시
    /// 코인을 구매할 당시의 가격 * (1 + SellRate)로 손절 가격을 결정함
    /// </summary>
    public int SellRate { get; set; }
    private StringBuilder LogStringBuilder { get; set; }

    public string ToLog()
    {
        LogStringBuilder ??= new StringBuilder();
        LogStringBuilder.Clear();
        LogStringBuilder.AppendLine("=============================");
        LogStringBuilder.AppendLine($"{nameof(MarketCode)}: {MarketCode}");
        LogStringBuilder.AppendLine($"{nameof(Symbol)}: {Symbol}");
        LogStringBuilder.AppendLine($"{nameof(TotalAmount)}: {TotalAmount}");
        LogStringBuilder.AppendLine($"{nameof(Amount)}: {Amount}");
        LogStringBuilder.AppendLine($"{nameof(BuyRate)}: {BuyRate}");
        LogStringBuilder.AppendLine($"{nameof(SellRate)}: {SellRate}");
        LogStringBuilder.AppendLine("=============================");
        
        return LogStringBuilder.ToString();
    }
}
