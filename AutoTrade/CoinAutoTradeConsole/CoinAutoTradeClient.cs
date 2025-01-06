using HttpService;
using SharedClass;

namespace CoinAutoTradeConsole;

public sealed class CoinAutoTradeClient(EMarketType marketType, string id, string ip, int port, string telegramApiToken, long telegramChatId)
    : HttpServiceClient($"{marketType}_{id}", ip, port, telegramApiToken, telegramChatId)
{
    public async Task<LoginResponse?> RequestLoginAsync()
    {
        var res = await RequestAsync<LoginRequest, LoginResponse>
            (EPacketType.Login, new LoginRequest());
        
        if (res == null)
            return null;

        Key = res.SymmetricKey;
        return res;
    }
    
    public async Task<AliveResponse?> RequestAliveAsync()
    {
        return await RequestAsync<AliveRequest, AliveResponse>
            (EPacketType.Alive, new AliveRequest());
    }
    
    public async Task<GetAllCoinTradeDataResponse?> RequestGetAllCoinTradeDataAsync()
    {
        return await RequestAsync<GetAllCoinTradeDataRequest, GetAllCoinTradeDataResponse>
            (EPacketType.GetAllCoinTradeData, new GetAllCoinTradeDataRequest());
    }
    
    public async Task<DeleteAllCoinTradeDataResponse?> RequestDeleteAllCoinTradeDataAsync()
    {
        return await RequestAsync<DeleteAllCoinTradeDataRequest, DeleteAllCoinTradeDataResponse>
            (EPacketType.DeleteAllCoinTradeData, new DeleteAllCoinTradeDataRequest());
    }

    public async Task<AddOrUpdateCoinTradeDataResponse?> RequestAddOrUpdateCoinTradeDataAsync(CoinTradeData coinTradeData)
    {
        return await RequestAsync<AddOrUpdateCoinTradeDataRequest, AddOrUpdateCoinTradeDataResponse>
            (EPacketType.AddOrUpdateCoinTradeData, new AddOrUpdateCoinTradeDataRequest(coinTradeData));
    }
    
    public async Task<GetCoinTradeDataResponse?> RequestGetCoinTradeDataAsync(string symbol)
    {
        return await RequestAsync<GetCoinTradeDataRequest, GetCoinTradeDataResponse>
            (EPacketType.GetCoinTradeData, new GetCoinTradeDataRequest(symbol));
    }
    
    public async Task<ResponseBody?> RequestDeleteCoinTradeDataAsync(string symbol)
    {
        return await RequestAsync<DeleteCoinTradeDataRequest, DeleteCoinTradeDataResponse>
            (EPacketType.DeleteCoinTradeData, new DeleteCoinTradeDataRequest(symbol));
    }
    
    public async Task<StartAllCoinTradeDataResponse?> RequestStartAllCoinTradeDataAsync(EMarketType marketType, string? apiKey, 
        string? secretKey, string? telegramApiToken, long telegramChatId)
    {
        var req = new StartAllCoinTradeDataRequest(marketType, apiKey, secretKey, telegramApiToken,telegramChatId);
        return await RequestAsync<StartAllCoinTradeDataRequest, StartAllCoinTradeDataResponse>(EPacketType.StartAllCoinAutoTrade, req);
    }
}