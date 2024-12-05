namespace SharedClass;

public enum EMarketType
{
    Bithumm,
}

public class UserMarketInfoRequest : RequestBody
{
    public EMarketType MarketType { get; set; }
    public string UserId { get; set; } = string.Empty;
}

public class UserMarketInfoResponse : ResponseBody
{
    public List<CoinTradeData> CoinTradeDataList { get; set; } = [];
}