using System.IdentityModel.Tokens.Jwt;

namespace CoinAutoTradeProcess;

public interface IMarket
{
    /// <summary>
    /// 마켓별로 사용하는 payload 정보가 다르기 때문에 개별 생성
    /// </summary>
    public JwtPayload GenerateJwtPayload();
    /// <summary>
    /// 마켓별로 사용하는 payload 정보가 다르기 때문에 개별 생성
    /// </summary>
    public JwtPayload GenerateJwtPayload(Dictionary<string, string> parameters);
    /// <summary>
    /// 마켓에서 지원하는 코인 정보를 정보가져옴
    /// </summary>
    public Task<MarketCodesResponse?> RequestMarketCodes();
    /// <summary>
    /// 현재 코인의 호가를 가져옴
    /// </summary>
    public Task<MarketTickerResponse?> RequestTicker(string marketCode);
    /// <summary>
    /// 현재 코인의 매도/매수 정보를 가져옴
    /// </summary>
    public Task<MarketOrderBookResponse?> RequestMarketOrderBook(string marketCode);
    /// <summary>
    /// 현재 계정에 보유 재화량을 가져옴
    /// </summary>
    public Task<MarketBalanceResponse?> RequestBalance(string symbol);
    /// <summary>
    /// 주문 uuid가 존재하는지 확인
    /// </summary>
    public Task<MarketOrderResponse?> RequestOrder(string uuid);
    /// <summary>
    /// 코인 매수 주문 (지정가 주문)
    /// </summary>
    public Task<MarketOrderResponse?> RequestBuy(string marketCode, double volume, double price);
    /// <summary>
    /// 코인 매도 주문 (시장가 매도)
    /// </summary>
    public Task<MarketOrderResponse?> RequestSell(string marketCode, double volume, double price); 
    /// <summary>
    /// 코인 주문 취소
    /// </summary>
    public Task<MarketOrderResponse?> RequestCancelOrder(string uuid); 
}