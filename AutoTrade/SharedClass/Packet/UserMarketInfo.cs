namespace SharedClass;

public enum EMarketType
{
    Bithumm,
}

public class UserMarketInfoResponse : ResponseBody
{
    public List<CoinTradeData>? CoinTradeDataList { get; set; } = new();
}