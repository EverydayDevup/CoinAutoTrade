using Newtonsoft.Json;
using SharedClass;
using EMarketType = SharedClass.EMarketType;

namespace CoinAutoTradeClient;

public partial class CoinAutoTradeConsole
{
    private enum EProcess : byte
    {
        SelectMarket = 0,
    }

    private static EMarketType MarketType { get; set; }
    private static string UserId { get; set; }
    private static LoggerService.LoggerService LoggerService { get; set; } = new();
    private static Dictionary<EProcess, Func<Task<bool>>> DicProcessFunc { get; set; } = new()
    {
        { EProcess.SelectMarket, SelectCoinAutoTradeMarket}
    };
    
    private static Queue<EProcess> ProcessQueue { get; set; } = new();

    public static async Task Main()
    {
        ProcessQueue.Enqueue(EProcess.SelectMarket);
        Update();
        
        var client = new CoinAutoTradeClient();
        await client.StartCoinAutoTradeAsync();
        Console.ReadLine();
    }

    private static async void Update()
    {
        while (true)
        {
            if (ProcessQueue.Count > 0)
            {
                var current = ProcessQueue.Dequeue();
                if (!DicProcessFunc.TryGetValue(current, out var func))
                    continue;
                
                var result = await func();
                if (result)
                    continue;
                
                LoggerService.ConsoleError($"{nameof(EProcess)} = {current} is error");
                break;
            }
            
            await Task.Delay(10);
        }
    }
}