namespace SharedClass;

public enum EPacketType
{
    Login,
    Alive, // 서버 상태 체크 
    GetAllCoinTradeData, // 코인 트레이드 정보
    DeleteAllCoinTradeData,
    AddOrUpdateCoinTradeData,
    GetCoinTradeData,
    DeleteCoinTradeData,
    StartAllCoinAutoTrade,
}

public enum EResponseCode
{
    Unknown = -1,
    Success = 0,
    DeleteAllCoinTradeDataFailed = 1000,
    AddCoinTradeDataFailed = 1001,
    DeleteCoinTradeDataFailed = 1002,
}

