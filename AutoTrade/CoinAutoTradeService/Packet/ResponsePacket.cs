using Newtonsoft.Json;

namespace CoinAutoTrade.Packet;

public class ResponsePacket<T, TK> where T : class where TK : class
{
    public Tuple<int, string> MakeResponse(string data)
    {
        var requestType = JsonConvert.DeserializeObject<T>(data);
        if (requestType == null)
            return new Tuple<int, string>()
        var response = MakeResponse(requestType);
        
    }

    protected virtual Tuple<int, TK> MakeResponse(T request)
    {
        return default;
    }
}