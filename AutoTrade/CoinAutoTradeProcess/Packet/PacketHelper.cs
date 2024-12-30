using RestSharp;

namespace AutoTrade.Packet;

public static class PacketHelper
{
    public static async Task<T?> RequestGetAsync<T>(RestClient client, RestRequest request) where T : IResponse, new()
    {
        try
        {
            var res = await client.ExecuteGetAsync(request);
            return Response<T>(res);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return default;
        }
    }
    
    public static async Task<T?> RequestDeleteAsync<T>(RestClient client, RestRequest request) where T : IResponse, new()
    {
        try
        {
            var res = await client.ExecuteDeleteAsync(request);
            return Response<T>(res);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return default;
        }
    }
    
    public static async Task<T?> RequestPostAsync<T>(RestClient client, RestRequest request) where T : IResponse, new()
    {
        try
        {
            var res = await client.ExecutePostAsync(request);
            return Response<T>(res);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return default;
        }
    }

    private static T Response<T>(RestResponse restResponse) where T : IResponse, new()
    {
        var response = new T();
        response.Parse(restResponse);
        return response;
    }
}