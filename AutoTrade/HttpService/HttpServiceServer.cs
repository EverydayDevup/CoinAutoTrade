using System.Net;
using System.Text;
using System.Text.Json;

namespace HttpService;

/// <summary>
/// http 서버를 구현하기 위해서 해당 클래스를 상속 받음
/// </summary>
public abstract class HttpServiceServer
{
    private readonly HttpServiceUrl _httpServiceUrl;
    private readonly HttpListener _listener = new ();
    private readonly LoggerService.LoggerService _loggerService = new ();

    public bool IsRunning => _listener.IsListening;

    protected virtual string LogDirectoryPath => $"{nameof(HttpServiceServer)}";

    protected HttpServiceServer(string ip, int port)
    {
        _httpServiceUrl = new HttpServiceUrl(ip, port);
        _listener.Prefixes.Add(_httpServiceUrl.Url);
        HttpServiceServerRun();
    }

    protected HttpServiceServer(int port)
    {
        _httpServiceUrl = new HttpServiceUrl(port);
        _listener.Prefixes.Add(_httpServiceUrl.Url);
        HttpServiceServerRun();
    }

    private void HttpServiceServerRun()
    {
        try
        {
            _listener.Start();
            _loggerService.ConsoleLog($"[{GetType().Name}] {nameof(HttpServiceServer)} is running : {_httpServiceUrl.Url}");

            while (true)
            {
                var context = _listener.GetContext();

                // 요청 처리
                var request = context.Request;

                if (request.HttpMethod.ToUpper().Equals(HttpServiceUtil.HttpMethod, StringComparison.CurrentCultureIgnoreCase))
                {
                    string requestBody;
                    using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                        requestBody = reader.ReadToEnd();

                    // JSON 파싱
                    var requestData = JsonSerializer.Deserialize<RequestData>(requestBody);
                    if (requestData != null)
                    {
                        var requestLogMessage =
                            $"[{GetType().Name}] Request : {nameof(requestData.Type)} = {requestData.Type}" +
                            $" {nameof(requestData.Body)} = {requestData.Body}";
                        
                        _loggerService.ConsoleLog(requestLogMessage);
                        _loggerService.FileLog(LogDirectoryPath, requestLogMessage);

                        var responseData = Parse(requestData);
                        var responseJson = JsonSerializer.Serialize(responseData);
                        
                        var buffer = Encoding.UTF8.GetBytes(responseJson);
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                        context.Response.ContentType = HttpServiceUtil.ContentType;
                        context.Response.ContentLength64 = buffer.Length;
                        context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                        context.Response.OutputStream.Close();

                        var responseLogMessage =
                            $"[{GetType().Name}] Response : {nameof(responseData.Type)} = {responseData.Type} " +
                            $"{nameof(responseData.Code)} = {responseData.Code} " +
                            $"{nameof(responseData.Body)} = {responseData.Body}";
                        
                        _loggerService.ConsoleLog(responseLogMessage);
                        _loggerService.FileLog(LogDirectoryPath, responseLogMessage);
                    }
                    else
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        context.Response.OutputStream.Close();
                        var requestErrorLogMessage =
                            $"{nameof(HttpServiceServerRun)} Request : {HttpStatusCode.BadRequest} {nameof(requestBody)} : {requestBody}";
                        
                        _loggerService.ConsoleError(requestErrorLogMessage);
                        _loggerService.FileLog(LogDirectoryPath, requestErrorLogMessage);
                    }
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                    context.Response.OutputStream.Close();
                    
                    var requestErrorLogMessage =
                        $"{nameof(HttpServiceServerRun)} Request : {HttpStatusCode.MethodNotAllowed} {nameof(request.HttpMethod)} : {request.HttpMethod}";
                    
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

    private ResponseData Parse(RequestData requestData)
    {
        var (code, body) = GenerateResponseData(requestData.Type, requestData.Body!);
        var response = new ResponseData(requestData.Type)
        {
            Code = code,
            Body = JsonSerializer.Serialize(new ResponseBodyData(body))
        };
        return response;
    }

    protected virtual Tuple<int, string> GenerateResponseData(int type, string data) => new (0, string.Empty);
}