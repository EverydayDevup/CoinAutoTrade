using System.Net;
using System.Text;
using System.Text.Json;
using SharedClass;

namespace HttpService;

/// <summary>
/// http 서버를 구현하기 위해서 해당 클래스를 상속 받음
/// </summary>
public abstract class HttpServiceServer
{
    private readonly HttpServiceUrl _httpServiceUrl;
    private readonly HttpListener _listener = new ();
    private readonly LoggerService.LoggerService _loggerService = new();

    private readonly Dictionary<string, string> _dicSymmetricKeys = new(); // Proxy 서버가 서비스되고 있는 중에는 Id 별로 대칭키를 유지함
    private readonly Dictionary<string, string> _dicSessions = new(); // 클라이언트가 중복으로 접속하는 것을 막기 위한 세션 발급
    
    private string _name = string.Empty;
    private string Name
    {
        get
        {
            if (string.IsNullOrEmpty(_name))
                _name = GetType().Name;

            return _name;
        }
    }

    private bool _isInit;
    
    // 각 프로토콜에서 요청에 따른 응답을 어떻게 처리할지 인터페이스를 통해 처리함
    protected Dictionary<EPacketType, IHttpServiceProtocol> DicHttpServiceProtocols { get; } = new();

    protected HttpServiceServer(string ip, int port)
    {
        _httpServiceUrl = new HttpServiceUrl(ip, port);
        _listener.Prefixes.Add(_httpServiceUrl.Url);
    }

    protected abstract void Init();

    public async Task HttpServiceServerRun()
    {
        try
        {
            if (!_isInit)
            {
                Init();
                _isInit = true;
            }
            
            _listener.Start();
            _loggerService.ConsoleLog($"[{Name}] Server is running : {_httpServiceUrl.Url}");

            while (true)
            {
                var context = await _listener.GetContextAsync();
                // 요청 처리
                var request = context.Request;
                
                if (request.HttpMethod.ToUpper().Equals(HttpServiceUtil.HttpMethod, StringComparison.CurrentCultureIgnoreCase))
                {
                    string requestBody;
                    using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                        requestBody = await reader.ReadToEndAsync();

                    // 요청 받은 데이터를 파싱해서, 어떤 응답을 보낼지 결정함
                    var requestData = JsonSerializer.Deserialize<RequestData>(requestBody);
                    if (requestData != null)
                    {
                        var requestLogMessage = $"[{Name}] Request : {requestData}";
                        
                        _loggerService.ConsoleLog(requestLogMessage);
                        _loggerService.FileLog(Name, requestLogMessage);
                        
                        var packetType = requestData.Type;
                        // 로그인을 새로하면 세션을 새로 발급
                        if (packetType == EPacketType.Login)
                        {
                            _dicSessions.Remove(requestData.Id);
                            
                            var sessionId = Guid.NewGuid().ToString();
                            _dicSessions.Add(requestData.Id, sessionId);
                            
                            var cookie = new Cookie("SessionId", sessionId);
                            {
                                cookie.Path = "/";
                                cookie.HttpOnly = true;
                            }
                            
                            context.Response.AppendCookie(cookie);
                        }
                        // 내부 통신이 아니라면 세션을 검증함
                        else if ((int)packetType < (int)EPacketType.InnerStartAllCoinAutoTrade)
                        {
                            var sessionId = request.Cookies.Count > 0 ? request.Cookies["SessionId"]?.Value : null;
                            var valid = sessionId != null;
                            
                            // 세션 비교를 해서 유효하지 않은 세션일 경우 에러 
                            if (!_dicSessions.TryGetValue(requestData.Id, out var id))
                                valid = false;

                            if (id != sessionId)
                                valid = false;

                            if (!valid)
                            {
                                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                context.Response.OutputStream.Close();

                                var sessionErrorLogMessage = "invalid session id";
                                _loggerService.ConsoleError(sessionErrorLogMessage);
                                _loggerService.FileLog(Name, sessionErrorLogMessage);
                                continue;
                            }
                        }
                        
                        var responseData = await MakeResponseDataAsync(requestData);
                        var responseJson = JsonSerializer.Serialize(responseData);
                       
                        if (HttpServiceUtil.IsCrypto(packetType) && _dicSymmetricKeys.TryGetValue(requestData.Id, out var key))
                            responseJson = Crypto.Encrypt(responseJson, key);
                      
                        var buffer = Encoding.UTF8.GetBytes(responseJson);
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                        context.Response.ContentType = HttpServiceUtil.ContentType;
                        context.Response.ContentLength64 = buffer.Length;
                        await context.Response.OutputStream.WriteAsync(buffer);
                        context.Response.OutputStream.Close();

                        var responseLogMessage = $"[{Name}] Response : {responseData}";
                        _loggerService.ConsoleLog(responseLogMessage);
                        _loggerService.FileLog(Name, responseLogMessage);
                    }
                    else
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        context.Response.OutputStream.Close();
                        var requestErrorLogMessage =
                            $"{Name} Request : {HttpStatusCode.BadRequest} {nameof(requestBody)} : {requestBody}";
                        
                        _loggerService.ConsoleError(requestErrorLogMessage);
                        _loggerService.FileLog(Name, requestErrorLogMessage);
                    }
                }
                // 요청 데이터가 정의되지 않은 경우
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                    context.Response.OutputStream.Close();
                    
                    var requestErrorLogMessage =
                        $"{Name} Request : {HttpStatusCode.MethodNotAllowed} {nameof(request.HttpMethod)} : {request.HttpMethod}";
                    
                    _loggerService.ConsoleError(requestErrorLogMessage);
                    _loggerService.FileLog(Name, requestErrorLogMessage);
                }
            }
        }
        catch (Exception ex)
        {
            var exceptionLogMessage = $"{nameof(HttpServiceServerRun)} Exception : {ex}";
            _loggerService.ConsoleError(exceptionLogMessage);
            _loggerService.FileLog(Name, exceptionLogMessage);
        }
        finally
        {
            _listener.Stop();
        }
    }

    private async Task<ResponseData> MakeResponseDataAsync(RequestData requestData)
    {
        var requestBody = requestData.Body;
        if (requestData.Type != (int)EPacketType.Login)
        {
            if (_dicSymmetricKeys.TryGetValue(requestData.Id, out var key))
                requestBody = Crypto.Decrypt(requestBody, key);
        }
                
        var (responseCode, responseBody) = await MakeResponseDataAsync(requestData.Type, requestData.Id, requestBody);
        
        var response = new ResponseData(requestData.Type)
        {
            Code = responseCode,
            Body = responseBody
        };
        
        return response;
    }

    private async Task<(EResponseCode, string?)> MakeResponseDataAsync(EPacketType type, string id, string? requestBody)
    {
        if (DicHttpServiceProtocols.TryGetValue(type, out var func))
            return await func.MakeResponseDataAsync(id, requestBody);

        return (EResponseCode.MakeResponseDataFailed, null);
    }

    public string? GetKey(string id)
    {
        return _dicSymmetricKeys.GetValueOrDefault(id);
    }
    
    public void SetKey(string id, string key)
    {
        _dicSymmetricKeys.Remove(id);
        _dicSymmetricKeys.TryAdd(id, key);
    }
}