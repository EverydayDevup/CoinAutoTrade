using CoinAutoTrade;
using HttpService;

namespace CoinAutoTradeClient;

public class CoinAutoTradeClient
{
    private readonly HttpServiceClient _serviceClient = new ("127.0.0.1", CoinAutoTradeService.Port);

    public async Task StartCoinAutoTradeAsync()
    {
        var result = await _serviceClient.Request<string, string>((int)ECoinAutoTradeRequestType.StartCoinAutoTrade, "Bithum");
        
    }
}