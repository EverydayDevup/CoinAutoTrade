using SharedClass;

namespace CoinAutoTradeProcess;

public static class MarketFactory
{
    /// <summary>
    /// 마켓 생성 정보
    /// </summary>
    private static readonly Dictionary<EMarketType, Func<string, string, IMarket>> DicMarketFactory = new ()
    {
        {EMarketType.Bithumb, (accessKey, secretKey) => new Bithumb(accessKey, secretKey)},
        {EMarketType.UpBit, (accessKey, secretKey) => new Upbit(accessKey, secretKey)}
    };

    public static IMarket? Create(EMarketType marketType, string accessKey, string secretKey)
    {
        return DicMarketFactory.TryGetValue(marketType, out var func) ? func.Invoke(accessKey, secretKey) : null;
    }
}