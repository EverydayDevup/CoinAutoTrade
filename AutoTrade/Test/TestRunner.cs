using System.Text;
using AutoTrade.Logic;
using AutoTrade.Market;
using AutoTrade.Packet.Common;

namespace AutoTrade.Test;

public static class TestRunner
{
    private static StringBuilder logStringBuilder = new();
    public static async Task Run(IMarket market)
    {
        Console.WriteLine("Starting tests...");
        Console.WriteLine(NotifyManager.GetLine());
        Console.WriteLine($"1. {nameof(market.RequestMarketOrderbook)} krw-xpla");
        var xpla = await market.RequestTicker("KRW-XPLA");
        Console.WriteLine($"1. {nameof(market.RequestMarketOrderbook)} krw-xpla Complete : {xpla}");
        Console.WriteLine(NotifyManager.GetLine());
        
        Console.WriteLine($"2. {nameof(market.RequestMarketOrderbook)} krw-xpla");
        Console.WriteLine(NotifyManager.GetLine());
        var orderBooks = await market.RequestMarketOrderbook("KRW-XPLA");
        Console.WriteLine(NotifyManager.GetLine());
        Console.WriteLine($"Buy OrderBook");
        Console.WriteLine($"{GetOrdersLog(orderBooks?.BuyOrders)}");
        Console.WriteLine(NotifyManager.GetLine());
        Console.WriteLine($"Sell OrderBook");
        Console.WriteLine($"{GetOrdersLog(orderBooks?.SellOrders)}");
        Console.WriteLine($"2.{nameof(market.RequestMarketOrderbook)}  krw-xpla complete");
        Console.WriteLine(NotifyManager.GetLine());
        
        Console.WriteLine($"3. {nameof(market.RequestBalance)} krw");
        Console.WriteLine(NotifyManager.GetLine());
        var krw = await market.RequestBalance("KRW");
        Console.WriteLine($"3. {nameof(market.RequestBalance)} krw : {krw}");
        Console.WriteLine(NotifyManager.GetLine());
        
        Console.WriteLine($"4. {nameof(market.RequestBuy)} KRW-XPLA / 250 / 20");
        Console.WriteLine(NotifyManager.GetLine());
        var buyUuid = await market.RequestBuy("KRW-XPLA", 250, 20);
        Console.WriteLine(string.IsNullOrEmpty(buyUuid)
            ? $"4. {nameof(market.RequestBuy)} fail"
            : $"4. {nameof(market.RequestBuy)} success {nameof(buyUuid)} : {buyUuid}");

        Console.WriteLine(NotifyManager.GetLine());

        if (string.IsNullOrEmpty(buyUuid))
            return;
        
        Console.WriteLine($"5. {nameof(market.RequestCheckOrder)} uuid");
        Console.WriteLine(NotifyManager.GetLine());
        var checkOrder = await market.RequestCheckOrder(buyUuid);
        Console.WriteLine($"5.{nameof(market.RequestCheckOrder)}  uuid complete : {checkOrder}");

        Console.WriteLine(NotifyManager.GetLine());
        
        Console.WriteLine($"6. {nameof(market.RequestCancelOrder)} uuid");
        Console.WriteLine(NotifyManager.GetLine());
        var deleteOrder = await market.RequestCancelOrder(buyUuid);
        Console.WriteLine($"6.{nameof(market.RequestCancelOrder)}  uuid complete : {deleteOrder}");
        Console.WriteLine(NotifyManager.GetLine());
    }

    private static string GetOrdersLog(MarketOrder[]? orders)
    {
        logStringBuilder.Clear();
        if (orders != null)
        {
            foreach (var order in orders)
                logStringBuilder.AppendLine($"[{nameof(order.Price)} : {order.Price} {nameof(order.Amount)} : {order.Amount}]");
        }

        return logStringBuilder.ToString();
    }
}