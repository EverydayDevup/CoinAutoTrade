namespace SharedClass;

public class CoinTradeDataRequest : RequestBody
{
    public CoinTradeData? CoinTradeData { get; init; }
}

public class CoinSymbolRequest : RequestBody
{
    public string Symbol { get; init; } = string.Empty;
}

public class StartAllCoinTradeDataRequest : RequestBody
{
    public int MarketType { get; init; }
    public string? ApiKey { get; init; } = string.Empty;
    public string? SecretKey { get; init; } = string.Empty;
    public string TelegramApiKey { get; init; } = string.Empty;
    public long TelegramChatId { get; init; }
}