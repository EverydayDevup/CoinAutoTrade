using HttpService;

namespace CoinAutoTradeService;

public static class CoinAutoTradeService
{
    public static async Task Main()
    {
        var coinAutoTradeServer = new CoinAutoTradeServer("*", HttpServiceUtil.CoinAutoTradeServicePort);
        await coinAutoTradeServer.HttpServiceServerRun();
    }
}