using AutoTrade.Logic;

Console.WriteLine("1. Create Market");
var market = MarketFactory.Create();

if (market == null)
{
    Console.WriteLine("Create Market Failed");
    return;
}

Console.WriteLine("2. Create Coin");
var coinConfigList = await CoinConfigFactory.CreateAsync(market);

if (coinConfigList == null)
{
    Console.WriteLine("Create Coin Failed");
    return;
}

Console.WriteLine("3. Trade Coin");

var tasks = new Task[coinConfigList.Count];
for (var i = 0; i < coinConfigList.Count; i++)
{
    tasks[i] = CoinTrade.Trade(market, coinConfigList[i]);
}

await Task.WhenAll(tasks);