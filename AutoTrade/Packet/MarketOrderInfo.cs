namespace AutoTrade.Packet;

public struct Orderbook
{
    public double Price { get; private set; }
    public double Amount { get; private set; }

    public Orderbook(double price, double amount)
    {
        Price = price;
        Amount = amount;
    }
}

public class MarketOrderInfo
{
    public Orderbook[]? BuyOrderbooks { get; set; }
    public Orderbook[]? SellOrderbooks { get; set; }
}