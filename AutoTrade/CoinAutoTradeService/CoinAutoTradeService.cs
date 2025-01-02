using HttpService;

namespace CoinAutoTradeService;

public static class CoinAutoTradeService
{
    public static void Main()
    {
        var coinAutoTradeServer = new CoinAutoTradeServer("*", HttpServiceUtil.CoinAutoTradeServicePort);
    }
}