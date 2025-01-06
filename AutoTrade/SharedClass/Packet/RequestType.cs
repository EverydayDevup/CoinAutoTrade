namespace SharedClass;

public class AliveRequest : RequestBody;
public class LoginRequest : RequestBody;
public class GetAllCoinTradeDataRequest : RequestBody;
public class DeleteAllCoinTradeDataRequest : RequestBody;
public class AddOrUpdateCoinTradeDataRequest(CoinTradeData coinTradeData) : RequestBody
{
    public CoinTradeData CoinTradeData { get; } = coinTradeData;
}
public class GetCoinTradeDataRequest(string symbol) : RequestBody
{
    public string Symbol { get; } = symbol;
}
public class DeleteCoinTradeDataRequest(string symbol) : RequestBody
{
    public string Symbol { get; } = symbol;
}


public class StartAllCoinTradeDataRequest(EMarketType marketType, string? apiKey, string? secretKey, string? telegramApiKey, long telegramChatId) : RequestBody
{
    public EMarketType MarketType { get; } = marketType;
    public string? ApiKey { get;  } = apiKey;
    public string? SecretKey { get;  } = secretKey;
    public string? TelegramApiKey { get; } = telegramApiKey;
    public long TelegramChatId { get; } = telegramChatId;
}


public class InnerStartAllCoinAutoTradeRequest(List<CoinTradeData>? coinTradeDataList) : RequestBody
{
    public List<CoinTradeData>? CoinTradeDataList { get; } = coinTradeDataList;
}

public class InnerAddOrUpdateCoinTradeDataRequest(CoinTradeData coinTradeData) : RequestBody
{
    public CoinTradeData CoinTradeData { get; } = coinTradeData;
}