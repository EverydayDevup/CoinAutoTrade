namespace SharedClass;

public enum EMarketType
{
    Bithumm,
}

public class UserMarketInfoRequest : RequestBody
{
    public EMarketType MarketType { get; set; }
    public string UserId { get; set; }
}

public class UserMarketInfoResponse : ResponseBody
{
    public List<CoinTradeData> CoinTradeDataList { get; set; }
}