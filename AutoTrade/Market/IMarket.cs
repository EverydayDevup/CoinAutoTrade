using System.IdentityModel.Tokens.Jwt;
using AutoTrade.Packet;

namespace AutoTrade.Market;

public interface IMarket
{
    /// <summary>
    /// 거래소에서 Open Api 사용 시 발급되는 키
    /// </summary>
    public string AccessKey { get; set; }
    /// <summary>
    /// 거래소에서 Open Api 사용 시 발급되는 키
    /// </summary>
    public string SecretKey { get; set; }
    /// <summary>
    /// 마켓에 해당 정보를 저장
    /// </summary>
    public void SetKey(string accessKey, string secretKey);
    /// <summary>
    /// 마켓별로 사용하는 payload 정보가 다르기 때문에 개별 생성
    /// </summary>
    public JwtPayload GenerateJwtPayload(Dictionary<string, string>? parameters);
    /// <summary>
    /// jwt 토큰 생성 후 header에 넣는 auth token으로 만듬
    /// </summary>
    public string GenerateAuthToken(Dictionary<string, string>? parameters);
    /// <summary>
    /// 현재 코인의 매도/매수 정보를 가져옴
    /// </summary>
    public Task<MarketOrderInfo?> RequestOrderbook(string marketCode);
    /// <summary>
    /// 현재 코인의 호가를 가져옴
    /// </summary>
    public Task<double> RequestTicker(string marketCode);
    /// <summary>
    /// 마켓에서 지원하는 코인 정보를 정보가져옴
    /// </summary>
    public Task<string[]?> RequestMarketCodes();
    /// <summary>
    /// 현재 계정에 보유 재화량을 가져옴
    /// </summary>
    public Task<long> RequestBalance(string coinSymbol);
    /// <summary>
    /// 주문 uuid가 존재하는지 확인
    /// </summary>
    public Task<bool> RequestCheckOrder(string uuid);
    /// <summary>
    /// 코인 매수 주문 (지정가 주문)
    /// </summary>
    public Task<string> RequestBuy(string coinSymbol, double volume, double price);
    /// <summary>
    /// 코인 매도 주문 (시장가 매도)
    /// </summary>
    public Task<string> RequestSell(string coinSymbol, double volume); 
    /// <summary>
    /// 코인 주문 취소
    /// </summary>
    public Task<bool> RequestCancelOrder(string uuid); 
}