using RestSharp;

namespace AutoTrade.Packet;

public interface IResponse
{
    public bool IsSuccess { get; set; }
    public void Parse(RestResponse response);
}