using System.Text;

namespace SharedClass;

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
    public int State { get; set; } = 0;
    /// <summary>
    /// 한번에 투자할 금액
    /// </summary>
    public double InvestRoundAmount { get; set; }
    /// <summary>
    /// 최초 구매 시의 가격 정보, 가격이 있을 경우 해당 가격이 되었을 때 주문하고 없을 경우 현재가로 구매함
    /// </summary>
    public double InitBuyPrice { get; set; }
    /// <summary>
    /// 해당 가격이 0 이상인 경우, 해당 금액 도달 시 보유한 코인 모두 판매
    /// </summary>
    public double MaxSellPrice { get; set; }
    /// <summary>
    /// 추가 매수 시 코인을 구매할 당시의 가격 * (1 + BuyRate) 로 다음 구매 가격을 결정함 
    /// </summary>
    public double RoundBuyRate { get; set; }
    /// <summary>
    /// 코인을 구매할 당시의 가격 * (1 + SellRate)로 손절 가격을 결정함
    /// </summary>
    public double RoundSellRate { get; set; }
    /// <summary>
    /// 손절 후 추가 매수를 통해 다시 코인 구매 프로세스를 진행할지의 여부
    /// 해당 값이 0 이상인 경우 해당 값 만큼 손절 후 구매를 시도함
    /// </summary>
    public double RebalancingMaxCount { get; set; }
    /// <summary>
    /// 손절 후 추가 구매 횟수
    /// </summary>
    public double RebalancingCount { get; set; }
    /// <summary>
    /// 코인 구매 가격
    /// </summary>
    public double BuyPrice { get; set; }
    /// <summary>
    /// 코인 구매 횟수
    /// </summary>
    public double BuyCount{ get; set; }
    /// <summary>
    /// 코인 판매 가격
    /// </summary>
    public double SellPrice { get; set; }
    private StringBuilder LogStringBuilder { get; } = new();

    public override string ToString()
    {
        LogStringBuilder.Clear();
        LogStringBuilder.AppendLine($"{nameof(Symbol)}: {Symbol}");
        LogStringBuilder.AppendLine($"{nameof(MarketCode)}: {MarketCode}");
        LogStringBuilder.AppendLine($"{nameof(State)}: {State}");
        LogStringBuilder.AppendLine($"{nameof(InvestRoundAmount)}: {InvestRoundAmount}");
        LogStringBuilder.AppendLine($"{nameof(InitBuyPrice)}: {InitBuyPrice}");
        LogStringBuilder.AppendLine($"{nameof(MaxSellPrice)}: {MaxSellPrice}");
        LogStringBuilder.AppendLine($"{nameof(RoundBuyRate)}: {RoundBuyRate}");
        LogStringBuilder.AppendLine($"{nameof(RoundSellRate)}: {RoundSellRate}");
        LogStringBuilder.AppendLine($"{nameof(RebalancingMaxCount)}: {RebalancingMaxCount}");
        LogStringBuilder.AppendLine($"{nameof(RebalancingCount)}: {RebalancingCount}");
        LogStringBuilder.AppendLine($"{nameof(BuyPrice)}: {BuyPrice}");
        LogStringBuilder.AppendLine($"{nameof(BuyCount)}: {BuyCount}");
        LogStringBuilder.AppendLine($"{nameof(SellPrice)}: {SellPrice}");
        
        return LogStringBuilder.ToString();
    }

    public string GetValidMessage()
    {
        if (string.IsNullOrEmpty(Symbol))
            return $"not found {nameof(Symbol)}";

        if (RoundBuyRate <= 0)
            return $"{nameof(RoundBuyRate)} is bigger than 0";

        if (RoundSellRate <= 0)
            return $"{nameof(RoundSellRate)} is bigger than 0";
        
        return string.Empty;
    }
}