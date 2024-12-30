namespace SharedClass;

public enum EMarketType
{
    Bithumm,
    UpBit,
}

public class GetAllCoinTradeDataResponse : ResponseBody
{
    public List<CoinTradeData>? CoinTradeDataList { get; set; } = new();
}