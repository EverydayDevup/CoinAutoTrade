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
    private string LogDirectoryPath => Path.Combine(Directory.GetCurrentDirectory(), Name);

    private readonly Dictionary<string, string> _dicKeys = new();
    private readonly Dictionary<string, string> _dicSessions = new();
    
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

    private bool _isInit = false;
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
                        _loggerService.FileLog(LogDirectoryPath, requestLogMessage);
                        
                        var responseData = await MakeResponseDataAsync(requestData);
                        var responseJson = JsonSerializer.Serialize(responseData);

                        if (requestData.Type == (int)EPacketType.Login)
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
                        else
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
                                
                                _loggerService.ConsoleError("invalid session id");
                            }
                            else
                            {
                                if (_dicKeys.TryGetValue(requestData.Id, out var key))
                                    responseJson = Crypto.Encrypt(responseJson, key);
                            }
                        }

                        var buffer = Encoding.UTF8.GetBytes(responseJson);
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                        context.Response.ContentType = HttpServiceUtil.ContentType;
                        context.Response.ContentLength64 = buffer.Length;
                        await context.Response.OutputStream.WriteAsync(buffer);
                        context.Response.OutputStream.Close();

                        var responseLogMessage = $"[{Name}] Response : {responseData}";
                        _loggerService.ConsoleLog(responseLogMessage);
                        _loggerService.FileLog(LogDirectoryPath, responseLogMessage);
                    }
                    else
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        context.Response.OutputStream.Close();
                        var requestErrorLogMessage =
                            $"{Name} Request : {HttpStatusCode.BadRequest} {nameof(requestBody)} : {requestBody}";
                        
                        _loggerService.ConsoleError(requestErrorLogMessage);
                        _loggerService.FileLog(LogDirectoryPath, requestErrorLogMessage);
                    }
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                    context.Response.OutputStream.Close();
                    
                    var requestErrorLogMessage =
                        $"{Name} Request : {HttpStatusCode.MethodNotAllowed} {nameof(request.HttpMethod)} : {request.HttpMethod}";
                    
                    _loggerService.ConsoleError(requestErrorLogMessage);
                    _loggerService.FileLog(LogDirectoryPath, requestErrorLogMessage);
                }
            }
        }
        catch (Exception ex)
        {
            var exceptionLogMessage = $"{nameof(HttpServiceServerRun)} Exception : {ex}";
            _loggerService.ConsoleError(exceptionLogMessage);
            _loggerService.FileLog(LogDirectoryPath, exceptionLogMessage);
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
            if (_dicKeys.TryGetValue(requestData.Id, out var key))
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
        return _dicKeys.GetValueOrDefault(id);
    }
    public void SetKey(string id, string key)
    {
        _dicKeys.Remove(id);
        _dicKeys.TryAdd(id, key);
    }
}