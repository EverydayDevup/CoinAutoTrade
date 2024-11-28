namespace HttpService;

/// <summary>
/// 서버에 요청할 데이터
/// </summary>
public class RequestData(int type, string body)
{
    public int Type { get; init; } = type;
    public string? Body { get; init; } = body;
}

/// <summary>
/// 클라이언트의 요청을 처리한 서버의 응답 값
/// </summary>
public class ResponseData(int type)
{
    public int Type { get; init; } = type;
    public int Code { get; set; } = 0;
    public string Body { get; set; }
}

public class ResponseBodyData(string data)
{
    public string Data { get; set; } = data;
}