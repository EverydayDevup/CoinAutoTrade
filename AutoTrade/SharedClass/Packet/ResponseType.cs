namespace SharedClass;

public class LoginResponse : ResponseBody
{
    public string SymmetricKey { get; init; } = string.Empty;
}

public class GetAllCoinTradeDataResponse : ResponseBody
{
    public List<CoinTradeData>? CoinTradeDataList { get; init; }
}

public class CoinTradeDataResponse : ResponseBody
{
    public CoinTradeData? CoinTradeData { get; set; }
}
