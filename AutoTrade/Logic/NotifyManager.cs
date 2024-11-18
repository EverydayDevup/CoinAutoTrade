namespace AutoTrade.Logic;

public static class NotifyManager
{
    public static void Notify(string message)
    {
        Console.WriteLine($"[LOG] {message}");
    }

    public static void NotifyError(string message)
    {
        Console.WriteLine($"[Error] {message}");
    }

    public static string GetLine()
    {
       return "=============================================";
    }
}