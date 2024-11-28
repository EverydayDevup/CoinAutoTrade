using System.Net;
using System.Text;
using System.Text.Json;

namespace HttpService;

/// <summary>
/// http 서버에 요청을 보내기 위한 클라이언트
/// </summary>
public class HttpServiceClient
{
    private readonly HttpServiceUrl _httpServiceUrl;
    private readonly LoggerService.LoggerService _loggerService = new ();

    public HttpServiceClient(string ip, int port)
    {
        _httpServiceUrl = new HttpServiceUrl(ip, port);
    }

    public HttpServiceClient(int port)
    {
        _httpServiceUrl = new HttpServiceUrl(port);
    }

    public async Task<T?> Request<T, TK>(int type, TK data)
    {
        using var client = new HttpClient();
        try
        {
            var requestData = new RequestData(type,  JsonSerializer.Serialize(data));
            var requestJson = JsonSerializer.Serialize(requestData);
            
            _loggerService.ConsoleLog($"[Request] : " +
                                        $"{nameof(requestData.Type)} = {requestData.Type} " +
                                        $"{nameof(requestData.Body)} = {requestData.Body}");
            
            var content = new StringContent(requestJson, Encoding.UTF8, HttpServiceUtil.ContentType);
            var response = await client.PostAsync(_httpServiceUrl.Url, content);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                _loggerService.ConsoleError($"[Response] Error : " +
                                            $"{nameof(response.StatusCode)} = {response.StatusCode}");

                return default;
            }

            // 응답 읽기
            var responseJson = await response.Content.ReadAsStringAsync();
            var responseData = JsonSerializer.Deserialize<ResponseData>(responseJson);
            
            if (responseData == null)
                return default;
            
            _loggerService.ConsoleLog($"[Response] : " +
                                        $"{nameof(responseData.Type)} = {responseData.Type} " +
                                        $"{nameof(responseData.Code)} = {responseData.Code} " +
                                        $"{nameof(responseData.Body)} = {responseData.Body} ");

            return responseData.Code != 0 ? default : JsonSerializer.Deserialize<T>(responseData.Body);
        }
        catch(Exception ex)
        {
            _loggerService.ConsoleError($"{nameof(HttpServiceClient)} : {ex.Message}");
            return default;
        }
    }
}