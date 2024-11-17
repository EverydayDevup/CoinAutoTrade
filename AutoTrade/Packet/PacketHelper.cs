using RestSharp;

namespace AutoTrade.Packet;
public static class PacketHelper
{
    public static async Task<T?> RequestGetAsync<T>(RestClient client, RestRequest request) where T : class, IResponse, new()
    {
        try
        {
            var restResponse = await client.GetAsync(request);
            return Response<T>(restResponse);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return null;
        }
    }
    
    public static async Task<T?> RequestDeleteAsync<T>(RestClient client, RestRequest request) where T : class, IResponse, new()
    {
        try
        {
            var restResponse = await client.DeleteAsync(request);
            return Response<T>(restResponse);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return null;
        }
    }
    
    public static async Task<T?> RequestPostAsync<T>(RestClient client, RestRequest request) where T : class, IResponse, new()
    {
        try
        {
            var restResponse = await client.PostAsync(request);
            return Response<T>(restResponse);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return null;
        }
    }

    private static T Response<T>(RestResponse restResponse) where T : class, IResponse, new()
    {
        var response = new T();
        response.Parse(restResponse);
        return response;
    }
}