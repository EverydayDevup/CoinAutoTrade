using System.Net;
using Newtonsoft.Json;
using RestSharp;

namespace CoinAutoTradeProcess;

public interface IResponse
{
    public bool IsSuccess { get; }
    public void Parse(RestResponse res);
}

public class Error
{
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonProperty("message")]
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// 응답을 처리하는 클래스
/// </summary>
/// <typeparam name="T"> string -> json으로 변경할 클래스</typeparam>
public abstract class Response<T> : IResponse where T : class
{
    public bool IsSuccess => Error != null;
    public Error? Error { get; private set; }
    public T? Result { get; private set; }
    
    public void Parse(RestResponse res)
    {
        if (res.ResponseStatus != ResponseStatus.Completed ||
            string.IsNullOrEmpty(res.Content))
        {
            if (!string.IsNullOrEmpty(res.Content))
                Error = JsonConvert.DeserializeObject<Error>(res.Content);
            
            return;
        }

        if (res.StatusCode == HttpStatusCode.OK || 
            res.StatusCode == HttpStatusCode.Created)
        {
            Result= JsonConvert.DeserializeObject<T>(res.Content);
            
            if (Result == null)
            {
                Error = new Error
                {
                    Name = "Unexpected response",
                    Message = $"{nameof(JsonConvert.DeserializeObject)} error",
                };
            }
        }
        else
        {
            Error = new Error
            {
                Name = "Unexpected Status Code",
                Message = $"{nameof(res.StatusCode)} {res.StatusCode}",
            };
        }
    }
}
