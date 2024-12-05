namespace SharedClass;

public enum EPacketType
{
    Login,
    Alive, // 서버 상태 체크 
    UserMarketInfo, // 사용자의 거래소 정보 
}

public enum EResponseCode
{
    Unknown = -1,
    Success = 0,
}

