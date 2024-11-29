namespace CoinAutoTrade;

public enum ECoinAutoTradeRequestType
{
    UserMarketInfo = 1,
    StartCoinAutoTrade = 2,
}

public enum ECoinAutoTradeResponseCode
{
    Success = 0,
    NotFoundRequestType,
    LoadCoinTradeDataListFail,
}