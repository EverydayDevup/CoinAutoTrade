using System.Net;
using AutoTrade.Logic;
using Newtonsoft.Json;
using RestSharp;

namespace AutoTrade.Packet;

public interface IResponse
{
    public bool IsSuccess { get; set; }
    public object? GetResult();
    public void Parse(RestResponse res);
}

/// <summary>
/// 응답을 처리하는 클래스
/// </summary>
/// <typeparam name="T"> string -> json으로 변경할 클래스</typeparam>
/// <typeparam name="TK"> 응답의 대한 결과</typeparam>
public class Response<T, TK> : IResponse
{
    public bool IsSuccess { get; set;}
    protected TK? Result { get; set; }

    public object? GetResult()
    {
        return Result;
    }

    public void Parse(RestResponse res)
    {
        IsSuccess = false;
        
        if (res.ResponseStatus != ResponseStatus.Completed ||
            string.IsNullOrEmpty(res.Content))
        {
            if (!string.IsNullOrEmpty(res.Content))
            {
                var template = new
                {
                    error = new
                    {
                        name = "",
                        message = ""
                    }
                };
                
                var result = JsonConvert.DeserializeAnonymousType(res.Content, template);
            }
            else
            {
            }
            
            return;
        }

        if (res.StatusCode == HttpStatusCode.OK || 
            res.StatusCode == HttpStatusCode.Created)
        {
            var obj = JsonConvert.DeserializeObject<T>(res.Content);
            // if (obj == null)
            //     MessageManager.Error($"{nameof(GetType)} - {nameof(T)} not fount deserialize type");
            // else
            // {
                IsSuccess = true;
                Parse(obj);
            //}
        }
    }

    protected virtual void Parse(T content)
    {
        return;
    }
}
