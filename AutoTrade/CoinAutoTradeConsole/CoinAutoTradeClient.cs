using HttpService;
using SharedClass;

namespace CoinAutoTradeClient;

public sealed class CoinAutoTradeClient : HttpServiceClient
{
    public CoinAutoTradeClient(EMarketType marketType, string id, string ip, int port, string telegramApiToken,
        long telegramChatId) : base(ip, port, telegramApiToken, telegramChatId)
    {
        Id = $"{marketType}_{id}_{DateTime.Now.Ticks}";
    }
     
    public async Task<bool> RequestLoginAsync()
    {
        var res = await Request<LoginInfoResponse, RequestBody>((int)EPacketType.Login, new RequestBody());
        if (res == null)
            return false;
        
        Key = res.Key;
        return true;
    }
    
    public async Task<bool> RequestAliveAsync()
    {
        var res = await Request<ResponseBody, RequestBody>((int)EPacketType.Alive, new RequestBody());
        return res != null;
    }
}