namespace AutoTrade.Market;

public interface IMarket
{
    public Task<string> RequestOrderbook(string marketCode);
    public Task<double> RequestTicker(string marketCode);
    public Task<string[]?> RequestMarketCodes();
}