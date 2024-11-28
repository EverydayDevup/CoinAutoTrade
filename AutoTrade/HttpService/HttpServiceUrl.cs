namespace HttpService;

/// <summary>
/// http 서버의 url을 관리
/// </summary>
internal struct HttpServiceUrl
{
    public string Url { get; private set; }
    
    public HttpServiceUrl(string ip, int port)
    {
        Url = $"http://{ip}:{port}/";
    }

    public HttpServiceUrl(int port)
    {
        Url = $"http://*:{port}/";
    }
}