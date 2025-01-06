using System.Text.Json;
using SharedClass;

namespace HttpService;

public interface IHttpServiceProtocol
{
    public Task<(EResponseCode, string?)> MakeResponseDataAsync(string id, string? requestBody);
}

public abstract class HttpServiceProtocol<T1, T2, T3>(T1 server) : IHttpServiceProtocol where T1 : HttpServiceServer where T2 : RequestBody where T3 : ResponseBody
{
    protected T1 Server { get; init; } = server;

    public async Task<(EResponseCode, string?)> MakeResponseDataAsync(string id, string? requestBody)
    {
        if (string.IsNullOrEmpty(requestBody))
            return (EResponseCode.RequestBodyIsNull, null);
            
        var request = JsonSerializer.Deserialize<T2>(requestBody);
        if (request == null)
            return (EResponseCode.SerializedFailedRequestBody, null);

        var (code, responseBody) = await MakeResponseDataAsync(id, request);
        return (code, JsonSerializer.Serialize(responseBody));
    }

    protected abstract Task<(EResponseCode, T3?)> MakeResponseDataAsync(string id, T2 request);
}