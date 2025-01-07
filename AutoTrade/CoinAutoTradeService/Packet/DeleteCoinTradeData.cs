﻿using HttpService;
using SharedClass;

namespace CoinAutoTradeService;

public class DeleteCoinTradeData(CoinAutoTradeServer server) : HttpServiceProtocol<HttpServiceServer, DeleteCoinTradeDataRequest, DeleteCoinTradeDataResponse>(server)
{
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    protected override async Task<(EResponseCode, DeleteCoinTradeDataResponse?)> MakeResponseDataAsync(string id, DeleteCoinTradeDataRequest request)
    {
        var result = CoinTradeDataManager.DeleteCoinTradeData(id, request.Symbol);
        if (!result)
            return (EResponseCode.DeleteCoinTradeDataFailed, null);

        if (!server.DicProcess.TryGetValue(id, out var value))
            return (EResponseCode.Success, new DeleteCoinTradeDataResponse());
        
        var (client, _) = value;
        result = await client.InnerRequestStartAllCoinAutoTradeAsync(CoinTradeDataManager.GetAllCoinTradeData(id));
        return !result ? (EResponseCode.DeleteCoinTradeDataFailed, null) : (EResponseCode.Success, new DeleteCoinTradeDataResponse());
    }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
}