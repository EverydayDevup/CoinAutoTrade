using HttpService;
using Newtonsoft.Json;
using SharedClass;

namespace CoinAutoTrade.Packet;

public class UserMarketInfo(CoinAutoTradeServer server) : HttpServiceProtocol<HttpServiceServer, RequestBody, UserMarketInfoResponse>(server)
{
    protected override Tuple<int, UserMarketInfoResponse?> MakeResponse(string id, RequestBody request)
    {
        var res = new UserMarketInfoResponse();
        
        var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), $"{nameof(UserMarketInfo)}");

        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);
        
        var filePath = Path.Combine(directoryPath, $"{id}.json");
        if (!File.Exists(filePath))
            return new Tuple<int, UserMarketInfoResponse?>((int)EResponseCode.Success, res);
        
        var json = File.ReadAllText(filePath);
        res.CoinTradeDataList = JsonConvert.DeserializeObject<List<CoinTradeData>>(json);
        
        return new Tuple<int, UserMarketInfoResponse?>((int)EResponseCode.Success, res);
    }
}