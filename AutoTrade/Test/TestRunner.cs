using AutoTrade.Market;

namespace AutoTrade.Test;

public static class TestRunner
{
    public static async Task Run(IMarket market)
    {
        Console.WriteLine("Starting tests...");
        Console.WriteLine("===========================");
        Console.WriteLine($"{nameof(market.RequestBalance)}");
        await market.RequestBalance("XPLA");
        Console.WriteLine($"{nameof(market.RequestCheckOrder)}");
        await market.RequestCheckOrder(Guid.NewGuid().ToString());
    }
}