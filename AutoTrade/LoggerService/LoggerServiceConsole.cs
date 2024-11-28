namespace LoggerService;

internal class LoggerServiceConsole
{
    private enum ELogType : byte
    {
        Log,
        Debug,
        Error,
    }
    
    private static string GetMessage(ELogType type, string message)
    {
        return $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}][{type.ToString()}] {message}";   
    }

    private static void WriteLine(ELogType type, string message)
    {
        Console.WriteLine(GetMessage(type, message));
    }
    
    public void Log(string message)
    {
        WriteLine(ELogType.Log, message);
    }
    
    public void Error(string message)
    {
        WriteLine(ELogType.Error, message);
    }

    public void Debug(string message)
    {
        WriteLine(ELogType.Debug, message);
    }
}