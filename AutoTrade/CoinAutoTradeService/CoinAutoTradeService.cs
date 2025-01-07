using HttpService;

namespace CoinAutoTradeService;

public static class CoinAutoTradeService
{
    public static async Task Main()
    {
        var coinAutoTradeServer = new CoinAutoTradeServer("54.180.225.18", HttpServiceUtil.CoinAutoTradeServicePort);
        await coinAutoTradeServer.HttpServiceServerRun();
    }
}