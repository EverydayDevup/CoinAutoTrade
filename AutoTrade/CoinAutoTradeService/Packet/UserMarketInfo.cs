namespace CoinAutoTrade.Packet;

public class UserMarketInfo : ResponsePacket<,>
{
    public Tuple<int, string> MakeResponse(string data)
    {
        return Tuple.Create<int, string>(1, data);
    }
}