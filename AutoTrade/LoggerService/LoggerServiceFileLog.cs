using System.Text;

namespace LoggerService;

internal class LoggerServiceFileLog(string directoryPath, int writeTimeMinutes)
{
    private readonly StringBuilder _stringBuilder = new();
    private DateTime _nextWriteTime = DateTime.MinValue;
    
    private static string GetMessage(string message)
    {
        return $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";   
    }

    public void Log(string message)
    {
        _stringBuilder.AppendLine(GetMessage(message));

        if (_nextWriteTime == DateTime.MinValue)
            _nextWriteTime = DateTime.Now.AddMinutes(writeTimeMinutes);

        if (_nextWriteTime >= DateTime.Now)
            return;
        
        Save();
    }

    public void Save()
    {
        try
        {
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
        
            var filePath = Path.Combine(directoryPath, $"{_nextWriteTime.AddMinutes(-writeTimeMinutes):yy-MM-dd_hh_mm_ss}~{_nextWriteTime:yy-MM-dd_hh_mm_ss}.log");
            File.WriteAllText(filePath, _stringBuilder.ToString());

            _stringBuilder.Clear();
            _nextWriteTime = DateTime.Now.AddMinutes(writeTimeMinutes);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}