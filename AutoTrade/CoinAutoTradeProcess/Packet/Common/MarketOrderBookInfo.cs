namespace CoinAutoTradeProcess;

public class MarketOrder(double price, double amount)
{
    public double Price { get; private set; } = price;
    public double Amount { get; private set; } = amount;
}

public class MarketOrderBook
{
    public MarketOrder[]? BuyOrders { get; set; }
    public MarketOrder[]? SellOrders { get; set; }
}