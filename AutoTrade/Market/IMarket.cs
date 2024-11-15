using AutoTrade.Packet;

namespace AutoTrade.Market;

public interface IMarket
{
    public Task<MarketOrderInfo?> RequestOrderbook(string marketCode);
    public Task<double> RequestTicker(string marketCode);
    public Task<string[]?> RequestMarketCodes();
}