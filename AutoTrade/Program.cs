//#define TEST_RUNNER

using AutoTrade.Logic;
using AutoTrade.Test;

Console.WriteLine("1. Market Config");
Console.WriteLine(MessageManager.GetLine());
// 사용할 거래소 선택
var marketType = MarketFactory.SelectMarketType();
// 거래소 정보를 새로 만들지, 불러올지 결정
var marketMenu = MarketFactory.SelectMarketMenu();
// 선택한 거래소의 정보에 따라 거래소를 불러옴
var market = MarketFactory.GetMarket(marketType, marketMenu);

if (market == null)
{
    MessageManager.Error($"{nameof(market)} : {marketType} is null");
    return;
}
Console.WriteLine("1. Market Config Complete");
Console.WriteLine(MessageManager.GetLine());

Console.WriteLine("2. Load Coin Config");
Console.WriteLine(MessageManager.GetLine());
var coinConfigList = await CoinConfigFactory.LoadAsync(market);

if (coinConfigList == null)
{
    Console.WriteLine("Load Coin Config");
    return;
}

Console.WriteLine("2. Load Coin Config Complete");
Console.WriteLine(MessageManager.GetLine());

Console.WriteLine("3. Trade Coin");
Console.WriteLine(MessageManager.GetLine());

#if TEST_RUNNER
    await TestRunner.Run(market);
#else
    await CoinTrade.Trade(market, coinConfigList);
#endif