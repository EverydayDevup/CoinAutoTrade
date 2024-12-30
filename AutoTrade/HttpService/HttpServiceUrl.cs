namespace HttpService;

/// <summary>
/// http 서버의 url을 관리
/// </summary>
internal struct HttpServiceUrl(string ip, int port)
{
    private string _url = string.Empty;

    public string Url
    {
        get
        {
            if (string.IsNullOrEmpty(_url))
                _url = $"http://{ip}:{port}/";

            return _url;
        }
    }

    private Uri _uri;

    public Uri Uri
    {
        get
        {
            if (_uri == null)
                _uri = new Uri(Url);
            
            return _uri;
        }
    }
}