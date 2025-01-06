namespace SharedClass;

/// <summary>
/// 서버에 요청할 데이터
/// </summary>
public class RequestData(EPacketType type, string id, string body)
{
    public EPacketType Type { get; init; } = type;
    public string Id { get; init; } = id; 
    public string Body { get; init; } = body;

    public override string ToString()
    {
        return $"{nameof(Type)} : {Type} {nameof(Id)} : {Id} {nameof(Body)} : {Body}";
    }
}

/// <summary>
/// 클라이언트의 요청을 처리한 서버의 응답 값
/// </summary>
public class ResponseData(EPacketType type)
{
    public EPacketType Type { get; } = type;
    public EResponseCode Code { get; init; }
    public string? Body { get; init; }

    public override string ToString()
    {
        return $"{nameof(Type)} : {Type} {nameof(Code)} : {Code} {nameof(Body)} : {Body}";
    }
}

/// <summary>
/// 서버에 요청하는 데이터 
/// </summary>
public abstract class RequestBody
{
    
}

/// <summary>
/// 서버에서 받은 응답 데이터
/// </summary>
public abstract class ResponseBody
{
    
}