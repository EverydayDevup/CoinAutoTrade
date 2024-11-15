using RestSharp;

namespace AutoTrade.Packet;
public static class PacketHelper
{
    public static async Task<T?> RequestAsync<T>(RestClient client, RestRequest request) where T : class, IResponse, new()
    {
        try
        {
            var restResponse = await client.GetAsync(request);
            var response = new T();
            response.Parse(restResponse);
            return response;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return null;
        }
    }
}