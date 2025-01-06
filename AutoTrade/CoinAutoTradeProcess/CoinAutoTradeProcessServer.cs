using System.Diagnostics;
using CoinAutoTradeProcess.Protocol;
using HttpService;
using SharedClass;

namespace CoinAutoTradeProcess;

public class CoinAutoTradeProcessServer(EMarketType marketType, string marketApiKey, string marketSecretKey, string ip, int port) : HttpServiceServer(ip, port)
{
    private EMarketType MarketType { get; } = marketType;
    private string MarketApiKey { get; } = marketApiKey;
    private string MarketSecretKey { get; } = marketSecretKey;
    public CoinAutoTrade? CoinAutoTrade { get; private set; }
    public IMarket? Market { get; private set; }
    
    protected override void Init()
    {
        Market = MarketFactory.Create(MarketType, MarketApiKey, MarketSecretKey);
        CoinAutoTrade = new CoinAutoTrade(Market);
        
        DicHttpServiceProtocols.Add(EPacketType.InnerStartAllCoinAutoTrade, new InnerStartAllCoinAutoTradeProtocol(this));
    }
}