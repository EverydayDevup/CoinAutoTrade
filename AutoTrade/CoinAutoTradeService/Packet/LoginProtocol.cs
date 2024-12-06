using System.Text;
using HttpService;
using SharedClass;

namespace CoinAutoTrade.Packet;

public class LoginProtocol(CoinAutoTradeServer server) : HttpServiceProtocol<HttpServiceServer, RequestBody, LoginResponse>(server)
{
    protected override Tuple<int, LoginResponse?> MakeResponse(string id, RequestBody request)
    {
        var key = $"{DateTime.Now.Ticks}_{GenerateRandomString(16)}";
        var res = new LoginResponse
        {
            Key = key
        };
        
        server.SetKey(id, key);
        return new Tuple<int, LoginResponse?>((int)EResponseCode.Success, res);
    }
    
    private string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var result = new StringBuilder(length);
        var random = new Random();

        for (var i = 0; i < length; i++)
            result.Append(chars[random.Next(chars.Length)]);

        return result.ToString();
    }
}