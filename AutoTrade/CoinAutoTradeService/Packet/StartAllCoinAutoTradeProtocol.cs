using System.Diagnostics;
using HttpService;
using SharedClass;

namespace CoinAutoTrade.Packet;

public class StartAllCoinAutoTradeProtocol(CoinAutoTradeServer server): HttpServiceProtocol<HttpServiceServer, RequestBody, ResponseBody>(server)
{
    protected override Tuple<int, ResponseBody?> MakeResponse(string id, RequestBody request)
    {
        if (server.DicProcess.TryGetValue(id, out var process))
            return new Tuple<int, ResponseBody?>((int)EResponseCode.Success, new ResponseBody());

        var port = HttpServiceUtil.GetAvailablePort(CoinAutoTradeService.Port + 1, CoinAutoTradeService.Port + 100000);
        var processStartInfo = new ProcessStartInfo
        {
            FileName = "CoinAutoTradeProcess.exe",
            Arguments = $"{port}", // 전달할 인자
            UseShellExecute = true, // 쉘 실행 비활성화
            CreateNoWindow = false // 콘솔 창 표시 안 함
        };

        using (process = new Process())
        {
            process.StartInfo = processStartInfo;
            process.Start();
        }
        
        server.DicProcess[id] = process;
        return new Tuple<int, ResponseBody?>((int)EResponseCode.Success, new ResponseBody()); 
    }
}