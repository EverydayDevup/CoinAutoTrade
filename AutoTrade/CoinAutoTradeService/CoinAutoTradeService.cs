using HttpService;

namespace CoinAutoTradeService;

public static class CoinAutoTradeService
{
    // 프로세스 시작 시 서버를 시작함
    public static async Task Main()
    {
        var coinAutoTradeServer = new CoinAutoTradeServer("*", HttpServiceUtil.CoinAutoTradeServicePort);
        await coinAutoTradeServer.HttpServiceServerRun();
    }
}