using AutoTrade.Packet;

namespace AutoTrade.Market;

public interface IMarket
{
    public string ApiKey { get; set; }
    public string SecretKey { get; set; }
    public void SetKey(string apiKey, string secretKey);
    public Task<MarketOrderInfo?> RequestOrderbook(string marketCode);
    public Task<double> RequestTicker(string marketCode);
    public Task<string[]?> RequestMarketCodes();
    public Task<long> RequestBalance(string currency);
}