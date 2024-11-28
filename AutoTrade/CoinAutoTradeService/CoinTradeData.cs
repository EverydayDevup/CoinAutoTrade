using System.Text;

namespace CoinAutoTrade;

public class CoinTradeData
{
    /// <summary>
    /// 코인 심볼 ex) BTC
    /// </summary>
    public string Symbol { get; set; } = string.Empty;
    /// <summary>
    /// 거래소에서 사용하는 마켓 코드 ex) KRW-BTC
    /// </summary>
    public string MarketCode => $"KRW-{Symbol}";
    /// <summary>
    /// 전체 투자 금액, 최대 투자 금액이 없을 경우 구매 횟수를 체크하지 않음
    /// </summary>
    public double InvestTotalAmount { get; set; }
    /// <summary>
    /// 최대 손실 비율
    /// </summary>
    public double MaxLossRate { get; set; }
    /// <summary>
    /// 한번에 투자할 금액
    /// </summary>
    public double InvestRoundAmount { get; set; }
    /// <summary>
    /// 최초 구매 시의 가격 정보, 가격이 있을 경우 해당 가격이 되었을 때 주문하고 없을 경우 현재가로 구매함
    /// </summary>
    public int InitBuyPrice { get; set; }
    /// <summary>
    /// 해당 가격이 0 이상인 경우, 해당 금액 도달 시 보유한 코인 모두 판매
    /// </summary>
    public int MaxSellPrice { get; set; }
    /// <summary>
    /// 추가 매수 시 코인을 구매할 당시의 가격 * (1 + BuyRate) 로 다음 구매 가격을 결정함 
    /// </summary>
    public int BuyRate { get; set; }
    /// <summary>
    /// 손절 타이밍 계산 시 첫 구매 때는 총 투자 금액의 1%를 기준으로 하고, 이후 구매 시
    /// 코인을 구매할 당시의 가격 * (1 + SellRate)로 손절 가격을 결정함
    /// </summary>
    public int SellRate { get; set; }
    private StringBuilder LogStringBuilder { get; } = new();

    public string ToLog()
    {
        LogStringBuilder.Clear();
        LogStringBuilder.AppendLine($"{nameof(Symbol)}: {Symbol}");
        LogStringBuilder.AppendLine($"{nameof(MarketCode)}: {MarketCode}");
        LogStringBuilder.AppendLine($"{nameof(InvestTotalAmount)}: {InvestTotalAmount}");
        LogStringBuilder.AppendLine($"{nameof(MaxLossRate)}: {MaxLossRate}");
        LogStringBuilder.AppendLine($"{nameof(InvestRoundAmount)}: {InvestRoundAmount}");
        LogStringBuilder.AppendLine($"{nameof(InitBuyPrice)}: {InitBuyPrice}");
        LogStringBuilder.AppendLine($"{nameof(MaxSellPrice)}: {MaxSellPrice}");
        LogStringBuilder.AppendLine($"{nameof(BuyRate)}: {BuyRate}");
        LogStringBuilder.AppendLine($"{nameof(SellRate)}: {SellRate}");
        
        return LogStringBuilder.ToString();
    }

    public string GetValidMessage()
    {
        if (string.IsNullOrEmpty(Symbol))
            return $"not found {nameof(Symbol)}";

        if (InvestTotalAmount > 0 && InvestRoundAmount > InvestTotalAmount)
            return $"{nameof(InvestRoundAmount)} is bigger than {nameof(InvestTotalAmount)}";

        if (MaxLossRate <= 0)
            return $"{nameof(MaxLossRate)} cannot be zero";
        
        return string.Empty;
    }
}