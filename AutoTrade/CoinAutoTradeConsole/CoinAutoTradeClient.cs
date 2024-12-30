﻿using HttpService;
using SharedClass;

namespace CoinAutoTradeConsole;

public sealed class CoinAutoTradeClient : HttpServiceClient
{
    public CoinAutoTradeClient(EMarketType marketType, string id, string ip, int port, string telegramApiToken,
        long telegramChatId) : base(ip, port, telegramApiToken, telegramChatId)
    {
        Id = $"{marketType}_{id}";
    }
     
    public async Task<bool> RequestLoginAsync()
    {
        var res = await Request<LoginResponse, RequestBody>((int)EPacketType.Login, new RequestBody());
        if (res == null)
            return false;
        
        Key = res.SymmetricKey;
        return true;
    }
    
    public async Task<bool> RequestAliveAsync()
    {
        var res = await Request<ResponseBody, RequestBody>((int)EPacketType.Alive, new RequestBody());
        return res != null;
    }
    
    public async Task<GetAllCoinTradeDataResponse?> RequestGetAllCoinTradeDataAsync()
    {
        var res = await Request<GetAllCoinTradeDataResponse, RequestBody>((int)EPacketType.GetAllCoinTradeData, new RequestBody());
        return res;
    }
    
    public async Task<ResponseBody?> RequestAddCoinTradeDataAsync(CoinTradeData coinTradeData)
    {
        var req = new CoinTradeDataRequest
        {
            CoinTradeData = coinTradeData
        };
        
        var res = await Request<ResponseBody, CoinTradeDataRequest>((int)EPacketType.AddOrUpdateCoinTradeData, req);
        return res;
    }
    
    public async Task<ResponseBody?> RequestDeleteAllCoinTradeDataAsync()
    {
        var res = await Request<ResponseBody, RequestBody>((int)EPacketType.DeleteAllCoinTradeData, new RequestBody());
        return res;
    }
    
    public async Task<CoinTradeDataResponse?> RequestGetCoinTradeDataAsync(string symbol)
    {
        var req = new CoinSymbolRequest
        {
            Symbol = symbol
        };
        
        var res = await Request<CoinTradeDataResponse, CoinSymbolRequest>((int)EPacketType.GetCoinTradeData, req);
        return res;
    }
    
    public async Task<ResponseBody?> RequestDeleteCoinTradeDataAsync(string symbol)
    {
        var req = new CoinSymbolRequest
        {
            Symbol = symbol
        };
        
        var res = await Request<ResponseBody, CoinSymbolRequest>((int)EPacketType.DeleteCoinTradeData, req);
        return res;
    }
    
    public async Task<ResponseBody?> RequestStartAllCoinTradeDataAsync(EMarketType marketType, string apiKey, string secretKey, string telegramApiToken, long telegramChatId)
    {
        var req = new StartAllCoinTradeDataRequest()
        {
            MarketType = (int)marketType,
            ApiKey = apiKey,
            SecretKey = secretKey,
            TelegramApiKey = telegramApiToken,
            TelegramChatId = telegramChatId
        };

        var res = await Request<ResponseBody, StartAllCoinTradeDataRequest>((int)EPacketType.StartAllCoinAutoTrade, req);
        return res;
    }
}