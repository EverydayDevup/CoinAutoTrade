using System.Net;
using System.Text;
using System.Text.Json;
using SharedClass;

namespace HttpService;

/// <summary>
/// http 서버에 요청을 보내기 위한 클라이언트
/// </summary>
public abstract class HttpServiceClient : IDisposable
{
    public string SymmetricKey { get; set; } = string.Empty; // 대치킹 암복호화에 사용하는 키
    protected string Id { get; init; } // 모든 통신의 키 값 
    private bool IsSending { get; set; } // 패킷 전송 중 여부, 모든 패킷은 한번에 하나씩 순차적으로 처리됨
    private byte RetryCount { get; set; } // 패킷 전송 실패 시 재전송 횟수 최대 MaxRetryCount 만큼 시도 후 실패 시 에러를 반환
    
    private const byte MaxRetryCount = 3;
    
    private readonly HttpClient _client;
    private readonly CookieContainer _cookieContainer = new ();
    private readonly HttpClientHandler _clientHandler = new ();
    
    private readonly HttpServiceUrl _httpServiceUrl;
    private bool _isDisposed;
    public LoggerService.LoggerService LoggerService { get; }
    
    protected HttpServiceClient(string id, string ip, int port, string? telegramApiToken, long telegramChatId)
    {
        Id = id;
        _httpServiceUrl = new HttpServiceUrl(ip, port);
        LoggerService = new LoggerService.LoggerService();
        
        if (!string.IsNullOrEmpty(telegramApiToken))
            LoggerService.SetTelegramInfo(GetType().Name, telegramApiToken, telegramChatId);

        _clientHandler.CookieContainer = _cookieContainer;
        _client = new HttpClient(_clientHandler);
    }

    protected async Task<TK?> RequestAsync<T, TK>(EPacketType type, T data, Action<EPacketType, EResponseCode>? failAction = null) where T : RequestBody where TK : ResponseBody 
    {
        try
        {
            while (IsSending)
                await Task.Delay(100);

            IsSending = true;
            RetryCount = 0;

            var requestBody = JsonSerializer.Serialize(data);
            
            LoggerService.ConsoleLog($"[Request] : " +
                                      $"Type = {type} " +
                                      $"{nameof(Id)} = {Id} " +
                                      $"Body = {requestBody}");
            
            // 패킷 암호화
            if (HttpServiceUtil.IsCrypto(type))
                requestBody = Crypto.Encrypt(requestBody, SymmetricKey);
            
            var requestData = new RequestData(type, Id, requestBody);
            var content = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, HttpServiceUtil.ContentType);
            var response = await _client.PostAsync(_httpServiceUrl.Url, content);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                if (response.StatusCode == HttpStatusCode.MethodNotAllowed ||
                    response.StatusCode == HttpStatusCode.BadRequest ||
                    response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    OnError($"[Response] Error : {nameof(ResponseData)} : {response}", type, EResponseCode.HttpStatusCodeError, failAction);
                    return default;
                }
                
                // 정의되지 않은 에러가 발생한 경우 패킷 재전송 시도 
                do
                {
                    RetryCount++;
                    response = await _client.PostAsync(_httpServiceUrl.Url, content);
                    LoggerService.ConsoleLog($"[Response] Fail : {nameof(RetryCount)} : {RetryCount}" + 
                                              $"{nameof(response.StatusCode)} = {response.StatusCode}");

                } while (response.StatusCode != HttpStatusCode.OK && RetryCount < MaxRetryCount);
            }

            // 최종 재시도 후에도 에러가 발생한 경우
            if (response.StatusCode != HttpStatusCode.OK)
            {
                OnError($"[Response] Error : {nameof(RetryCount)} : {RetryCount} " +
                        $"{nameof(response.StatusCode)} = {response.StatusCode}", type, EResponseCode.HttpRequestRetryOver, failAction);
                
                return default;
            }

            // 응답 읽기
            var responseJson = await response.Content.ReadAsStringAsync();
            
            // 패킷 복호화
            if (HttpServiceUtil.IsCrypto(type))
                responseJson = Crypto.Decrypt(responseJson, SymmetricKey);
            
            var responseData = JsonSerializer.Deserialize<ResponseData>(responseJson);
            
            if (responseData != null && !string.IsNullOrEmpty(responseData.Body))
            {
                LoggerService.ConsoleLog($"[Response] : " +
                                          $"Type = {responseData.Type} " +
                                          $"Code = {responseData.Code} " +
                                          $"Body = {responseData.Body} ");
                
                if (responseData.Code == (int)EResponseCode.Success)
                {
                    IsSending = false;
                    return JsonSerializer.Deserialize<TK>(responseData.Body);
                }
            }

            if (responseData == null)
                OnError($"[Response] Error : {nameof(responseData)} is null", type, EResponseCode.SerializedFailedResponseData, failAction);
            else
                OnError($"[Response] Error : {nameof(responseData.Code)} = {responseData.Code}", type, responseData.Code, failAction);
            
            return default;
        }
        catch (Exception ex)
        {
            OnError($"{nameof(HttpServiceClient)} : {ex.Message}", type, EResponseCode.HttpRequestException, failAction);
            return default;
        }
    }

    private async void OnError(string message, EPacketType type, EResponseCode code, Action<EPacketType, EResponseCode>? failAction = null)
    {
        IsSending = false;
        LoggerService.ConsoleError(message);
        await LoggerService.TelegramLogAsync(message);
        failAction?.Invoke(type, code);
    }

    public void Dispose()
    {
        if (_isDisposed)
            return;

        _isDisposed = true;
        _client.Dispose();
        
        GC.SuppressFinalize(this);
    }

    ~HttpServiceClient()
    {
        Dispose();
    }
}