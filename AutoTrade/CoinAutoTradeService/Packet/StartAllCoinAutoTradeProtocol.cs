using System.Diagnostics;
using HttpService;
using SharedClass;

namespace CoinAutoTradeService;

public class StartAllCoinAutoTradeProtocol(CoinAutoTradeServer server): HttpServiceProtocol<HttpServiceServer, StartAllCoinTradeDataRequest, StartAllCoinTradeDataResponse>(server)
{
    protected override async Task<(EResponseCode, StartAllCoinTradeDataResponse?)> MakeResponseDataAsync(string id, StartAllCoinTradeDataRequest request)
    {
        if (server.DicProcess.TryGetValue(id, out var value))
        {
            if (value.Item2.HasExited)
                server.DicProcess.Remove(id);
        }
        
        if (!server.DicProcess.TryGetValue(id, out value))
        {
            var port = HttpServiceUtil.GetAvailablePort(HttpServiceUtil.CoinAutoTradeServicePort + 1, HttpServiceUtil.CoinAutoTradeServicePort + 100000);
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "CoinAutoTradeProcess.exe",
                Arguments = $"{port} {(int)request.MarketType} {request.ApiKey} {request.SecretKey} {request.TelegramApiKey} {request.TelegramChatId} {Process.GetCurrentProcess().Id}", // 전달할 인자
                UseShellExecute = true, // 쉘 실행 비활성화
                CreateNoWindow = false // 콘솔 창 표시 안 함
            };

            value.Item1 = new CoinAutoTradeClient(id,HttpServiceUtil.LocalHost, port, request.TelegramApiKey, request.TelegramChatId);
            value.Item2 = new Process();
            value.Item2.StartInfo = processStartInfo;
            value.Item2.Start();
            server.DicProcess[id] = value;
        }

        var allCoinTradeData = CoinTradeDataManager.GetAllCoinTradeData(id);
        if (allCoinTradeData == null)
            return (EResponseCode.InnerStartAllCoinAutoTradeFailedNotFoundTradeData, null);
        
        var res = await value.Item1.InnerRequestStartAllCoinAutoTradeAsync(allCoinTradeData);
        return !res ? (EResponseCode.InnerStartAllCoinAutoTradeFailed, null) : (EResponseCode.Success, new StartAllCoinTradeDataResponse());
    }
}