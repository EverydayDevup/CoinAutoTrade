namespace SharedClass;

public class AliveResponse : ResponseBody;
public class LoginResponse(string symmetricKey) : ResponseBody
{
    public string SymmetricKey { get; } = symmetricKey;
}
public class GetAllCoinTradeDataResponse(List<CoinTradeData>? coinTradeDataList) : ResponseBody
{
    public List<CoinTradeData>? CoinTradeDataList { get; } = coinTradeDataList;
}
public class DeleteAllCoinTradeDataResponse : ResponseBody;
public class AddOrUpdateCoinTradeDataResponse : ResponseBody;
public class GetCoinTradeDataResponse(CoinTradeData? coinTradeData) : ResponseBody
{
    public CoinTradeData? CoinTradeData { get; } = coinTradeData;
}
public class DeleteCoinTradeDataResponse : ResponseBody;
public class StartAllCoinTradeDataResponse : ResponseBody;
public class InnerStartAllCoinAutoTradeResponse: ResponseBody;
public class InnerAddOrUpdateCoinTradeDataResponse : ResponseBody;
