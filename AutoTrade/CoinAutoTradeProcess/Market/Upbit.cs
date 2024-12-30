using System.IdentityModel.Tokens.Jwt;

namespace CoinAutoTradeProcess;

public class Upbit(string accessKey, string secretKey) : Market(accessKey, secretKey), IMarket
{
    private IMarket _marketImplementation;
    public JwtPayload GenerateJwtPayload()
    {
        return _marketImplementation.GenerateJwtPayload();
    }

    public JwtPayload GenerateJwtPayload(Dictionary<string, string> parameters)
    {
        return _marketImplementation.GenerateJwtPayload(parameters);
    }

    public Task<string[]?> RequestMarketCodes()
    {
        return _marketImplementation.RequestMarketCodes();
    }

    public Task<double> RequestTicker(string marketCode)
    {
        return _marketImplementation.RequestTicker(marketCode);
    }

    public Task<MarketOrderBook?> RequestMarketOrderbook(string marketCode)
    {
        return _marketImplementation.RequestMarketOrderbook(marketCode);
    }

    public Task<double> RequestBalance(string coinSymbol)
    {
        return _marketImplementation.RequestBalance(coinSymbol);
    }

    public Task<bool> RequestCheckOrder(string uuid)
    {
        return _marketImplementation.RequestCheckOrder(uuid);
    }

    public Task<string?> RequestBuy(string marketCode, double volume, double price)
    {
        return _marketImplementation.RequestBuy(marketCode, volume, price);
    }

    public Task<string?> RequestSell(string marketCode, double volume, double price)
    {
        return _marketImplementation.RequestSell(marketCode, volume, price);
    }

    public Task<bool> RequestCancelOrder(string uuid)
    {
        return _marketImplementation.RequestCancelOrder(uuid);
    }
}