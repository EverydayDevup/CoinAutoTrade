namespace SharedClass;

public enum EMarketType
{
    Bithumm,
}

public class GetAllCoinTradeDataResponse : ResponseBody
{
    public List<CoinTradeData>? CoinTradeDataList { get; set; } = new();
}