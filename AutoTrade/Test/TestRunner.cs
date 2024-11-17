using AutoTrade.Market;

namespace AutoTrade.Test;

public static class TestRunner
{
    public static async Task Run(IMarket market)
    {
        Console.WriteLine("Starting tests...");
        Console.WriteLine("===========================");
        Console.WriteLine($"{nameof(market.RequestBuy)}");
        var krw = await market.RequestBuy("KRW-XPLA", 550, 10);
    }
}