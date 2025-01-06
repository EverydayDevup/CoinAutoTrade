using System.Text;
using HttpService;
using SharedClass;

namespace CoinAutoTradeService;

public class LoginProtocol(CoinAutoTradeServer server) : HttpServiceProtocol<HttpServiceServer, LoginRequest, LoginResponse>(server)
{
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    protected override async Task<(EResponseCode, LoginResponse?)> MakeResponseDataAsync(string id, LoginRequest request)
    {
        var key = server.GetKey(id);
        if (!string.IsNullOrEmpty(key)) 
            return (EResponseCode.Success, new LoginResponse(key));
        
        key = $"{DateTime.Now.Ticks}_{GenerateRandomString(16)}";
        key = Crypto.GetSha256Hash(key);
        server.SetKey(id, key);

        return (EResponseCode.Success, new LoginResponse(key));
    }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    
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