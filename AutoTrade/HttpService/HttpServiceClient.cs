using System.Net;
using System.Text;
using System.Text.Json;
using SharedClass;

namespace HttpService;

/// <summary>
/// http 서버에 요청을 보내기 위한 클라이언트
/// </summary>
public class HttpServiceClient
{
    private readonly HttpServiceUrl _httpServiceUrl;
    private readonly LoggerService.LoggerService _loggerService;
    private bool IsSending { get; set; } = false;
    private byte RetryCount { get; set; } = 0;
    private byte MaxRetryCount { get; set; } = 0;
    protected string Key { get; set; } = string.Empty;
    protected virtual string Id { get; set; }

    protected HttpServiceClient(string ip, int port, string telegramApiToken, long telegramChatId)
    {
        _loggerService = new LoggerService.LoggerService(telegramApiToken, telegramChatId);
        _httpServiceUrl = new HttpServiceUrl(ip, port);
    }

    protected HttpServiceClient(int port, string telegramApiToken, long telegramChatId)
    {
        _loggerService = new LoggerService.LoggerService(telegramApiToken, telegramChatId);
        _httpServiceUrl = new HttpServiceUrl(port);
    }

    public async Task<T?> Request<T, TK>(int type, TK data, Action<int, int>? failAction = null) where T : ResponseBody where TK : RequestBody
    {
        using var client = new HttpClient();
        try
        {
            while (IsSending)
                await Task.Delay(100);

            IsSending = true;
            RetryCount = 0;

            var requestBody = JsonSerializer.Serialize(data);
            if (type != (int)EPacketType.Login)
                requestBody = Crypto.Encrypt(requestBody, Key);
            
            var requestData = new RequestData(type, Id, requestBody);
            var requestJson = JsonSerializer.Serialize(requestData);
            
            _loggerService.ConsoleLog($"[Request] : " +
                                        $"{nameof(requestData.Type)} = {requestData.Type} " +
                                        $"{nameof(requestData.Id)} = {requestData.Id} " +
                                        $"{nameof(requestData.Body)} = {requestData.Body}");
            
            var content = new StringContent(requestJson, Encoding.UTF8, HttpServiceUtil.ContentType);
            var response = await client.PostAsync(_httpServiceUrl.Url, content);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                // 3번 재시도 후에도 에러가 났다면 실패처리
                do
                {
                    RetryCount++;
                    response = await client.PostAsync(_httpServiceUrl.Url, content);
                    _loggerService.ConsoleLog($"[Response] Fail : {nameof(RetryCount)} : {RetryCount}" + 
                                              $"{nameof(response.StatusCode)} = {response.StatusCode}");

                } while (response.StatusCode != HttpStatusCode.OK && RetryCount < MaxRetryCount);
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                OnError($"[Response] Error : {nameof(RetryCount)} : {RetryCount} " +
                        $"{nameof(response.StatusCode)} = {response.StatusCode}", type, (int)EResponseCode.Unknown, failAction);
                return default;
            }

            // 응답 읽기
            var responseJson = await response.Content.ReadAsStringAsync();
            
            if (type != (int)EPacketType.Login)
                responseJson = Crypto.Decrypt(responseJson, Key);
            
            var responseData = JsonSerializer.Deserialize<ResponseData>(responseJson);
            
            if (responseData != null && !string.IsNullOrEmpty(responseData.Body))
            {
                _loggerService.ConsoleLog($"[Response] : " +
                                          $"{nameof(responseData.Type)} = {responseData.Type} " +
                                          $"{nameof(responseData.Code)} = {responseData.Code} " +
                                          $"{nameof(responseData.Body)} = {responseData.Body} ");
                
                IsSending = false;
                return responseData.Code != (int)EResponseCode.Success ? default : JsonSerializer.Deserialize<T>(responseData.Body);
            }

            if (responseData == null)
            {
                OnError($"[Response] Error : {nameof(responseData)} is null", type, (int)EResponseCode.Unknown, failAction);
            }
            else
            {
                OnError($"[Response] Error : {nameof(responseData.Code)} = {responseData.Code}", type, responseData.Code, failAction);
            }
            
            IsSending = false;
            return default;
        }
        catch (Exception ex)
        {
            OnError($"{nameof(HttpServiceClient)} : {ex.Message}", type, (int)EResponseCode.Unknown, failAction);
            return default;
        }
    }

    private async void OnError(string message, int type, int code, Action<int, int>? failAction = null)
    {
        IsSending = false;
        _loggerService.ConsoleError(message);
        await _loggerService.TelegramAsync(message);
        failAction?.Invoke(type, code);
    }
}