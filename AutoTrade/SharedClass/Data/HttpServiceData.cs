namespace SharedClass;

/// <summary>
/// 서버에 요청할 데이터
/// </summary>
public class RequestData(int type, string id, string body)
{
    public int Type { get; init; } = type;
    public string Id { get; init; } = id; 
    public string? Body { get; init; } = body;
}

/// <summary>
/// 클라이언트의 요청을 처리한 서버의 응답 값
/// </summary>
public class ResponseData(int type)
{
    public int Type { get; init; } = type;
    public int Code { get; init; } = 0;
    public string? Body { get; init; } = string.Empty; 
}

/// <summary>
/// 서버에 요청하는 데이터 
/// </summary>
public class RequestBody
{
    
}

/// <summary>
/// 서버에서 받은 응답 데이터
/// </summary>
public class ResponseBody
{
    
}