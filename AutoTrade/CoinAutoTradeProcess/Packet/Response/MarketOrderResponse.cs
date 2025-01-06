using Newtonsoft.Json;

namespace CoinAutoTradeProcess;

public enum EMarketOrderState
{
    Wait, // 체결대기
    Watch, // 예약 주문 대기
    Done, // 전체 체결 완료
    Cancel, // 주문 취소
    None,
}

public class MarketOrderJson
{
    [JsonProperty("uuid")] 
    public string? Uuid { get; set; }
    [JsonProperty("side")] 
    public string? Side { get; set; }
    [JsonProperty("state")] 
    public string? State { get; set; }
    [JsonProperty("volume")] 
    public string? Volume { get; set; }
    [JsonProperty("remaining_volume")] 
    public string? RemainingVolume { get; set; }
    [JsonProperty("executed_volume")] 
    public string? ExecutedVolume { get; set; }

    public EMarketOrderState GetState()
    {
        return State?.ToUpper() switch
        {
            "WAIT" => EMarketOrderState.Wait,
            "WATCH" => EMarketOrderState.Watch,
            "DONE" => EMarketOrderState.Done,
            "CANCEL" => EMarketOrderState.Cancel,
            _ => EMarketOrderState.None
        };
    }

    public double GetVolume()
    {
        if (!string.IsNullOrEmpty(Volume)) 
            return double.Parse(Volume);

        return 0;
    }
    
    public double GetRemainingVolume()
    {
        if (!string.IsNullOrEmpty(RemainingVolume)) 
            return double.Parse(RemainingVolume);

        return 0;
    }
    
    public double GetExecutedVolume()
    {
        if (!string.IsNullOrEmpty(ExecutedVolume)) 
            return double.Parse(ExecutedVolume);

        return 0;
    }
}

public class MarketOrderResponse : Response<MarketOrderJson>
{
    
}