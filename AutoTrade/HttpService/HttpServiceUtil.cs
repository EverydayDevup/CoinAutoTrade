using System.Net;

namespace HttpService;

public static class HttpServiceUtil
{
    public const string HttpMethod = "POST";
    public const string ContentType = "application/json";
    public const int CoinAutoTradeServicePort = 50000;
    public const string LocalHost = "127.0.0.1";
    
    /// <summary>
    /// http 서버를 올릴 때 사용할 포트를 찾음
    /// </summary>
    public static int GetAvailablePort(int startPort, int endPort)
    {
        for (var port = startPort; port <= endPort; port++)
        {
            if (IsPortAvailable(port))
                return port;
        }
        
        return -1;

        bool IsPortAvailable(int port)
        {
            try
            {
                using var listener = new HttpListener();
                listener.Prefixes.Add($"http://{LocalHost}:{port}/");
                listener.Start();
                listener.Stop(); 
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}