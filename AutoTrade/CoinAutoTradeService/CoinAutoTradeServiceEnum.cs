namespace CoinAutoTrade;

public enum ECoinAutoTradeRequestType
{
    StartCoinAutoTrade = 1,
}

public enum ECoinAutoTradeResponseCode
{
    Success = 0,
    NotFoundRequestType,
    LoadCoinTradeDataListFail,
}