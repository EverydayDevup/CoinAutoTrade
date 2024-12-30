using HttpService;
using SharedClass;

namespace CoinAutoTradeProcess;

public class CoinAutoTradeProcessServer(EMarketType marketType, string marketApiKey, string marketSecretKey, string ip, int port) : HttpServiceServer(ip, port)
{
    private EMarketType MarketType { get; set; } = marketType;
    private string MarketApiKey { get; set; } = marketApiKey;
    private string MarketSecretKey { get; set; } = marketSecretKey;
    public CoinAutoTrade? CoinAutoTrade { get; private set; }
    private IMarket? _market;
    
    protected override void Init()
    {
       _market = MarketFactory.Create(MarketType, MarketApiKey, MarketSecretKey);
       if (_market != null)
           CoinAutoTrade = new CoinAutoTrade(_market);
    }
}