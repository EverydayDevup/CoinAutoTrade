namespace SharedClass;

public enum EMarketType : byte
{
    Bithumb,
    UpBit,
}

public enum ECoinTradeType : byte
{
    AutoTrade, // 일반적인 자동 매매
    Pumping, // 펌핑으로 추가된 코인
    NewCoin, // 신규 상장 코인
}

public enum ECoinTradeState : byte
{
    Ready,
    Progress,
    Completed,
    Stop
}

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
    
    InnerStartAllCoinAutoTrade = 1000,
    InnerAddOrUpdateCoinTradeData
}

public enum EResponseCode
{
    Success = 0,

#region HTTP_ERROR
    HttpRequestException = 1000, // http 요청을 보내는 중 예외가 발생한 경우
    HttpStatusCodeError, // http 통신에서 오류가 발생한 경우
    HttpRequestRetryOver, // http 통신의 최대 재시도 횟수가 초가된 경우
    MakeResponseDataFailed, // 응답 데이터를 만드는 중 에러가 발생한 경우 
    RequestBodyIsNull, // 요청 데이터가 없는 경우
    SerializedFailedRequestBody, // 요청 데이터를 파싱하다가 에러가 발생한 경우
    SerializedFailedResponseData, // 응답 데이터를 파싱 하다가 에러가 발생한 경우
#endregion HTTP_ERROR

#region OUTBOUND_ERROR
    DeleteAllCoinTradeDataFailed = 2000, // 전체 코인 데이터를 삭제하는 중 예외가 발생한 경우
    AddCoinTradeDataFailed, // 코인 데이터를 추가 및 갱신 하는 중 에외가 발생한 경우 
    DeleteCoinTradeDataFailed, // 코인 데이터를 삭제하는 중 예외가 발생한 경우
#endregion OUTBOUND_ERROR

#region INBOUND_ERROR
    InnerStartAllCoinAutoTradeFailedNotFoundTradeData = 3000, // 코인 매매를 실행하려 했지만, 코인 거래 정보가 없는 경우 
    InnerStartAllCoinAutoTradeFailedNotFoundProcess, // 코인 매매를 실행하려 했지만, 코인 거래 프로세스가 없는 경우 
    InnerStartAllCoinAutoTradeFailedNotFoundSymmetricKeyError, // 코인 매매를 실행할려고 했지만, 대칭키가 없는 경우
    InnerAddCoinTradeDataFailed, // 내부 통신에서 코인 정보 업데이트 시 에러가 발생한 경우

#endregion INBOUND_ERROR
}

