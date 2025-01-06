using System.Text;

namespace LoggerService;

/// <summary>
/// 파일로 남기는 로그를 관리
/// </summary>
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
    }

    public void Save()
    {
        try
        {
            if (_nextWriteTime == DateTime.MinValue)
                _nextWriteTime = DateTime.Now.AddMinutes(writeTimeMinutes);
            
            if (_nextWriteTime >= DateTime.Now)
                return;
            
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
            
            var files = Directory.GetFiles(directoryPath);
            var deleteTimeTicks = DateTime.Now.AddMinutes(-60 * 24);
            for (var i = files.Length - 1; i >= 0; i--)
            {
                var fileName = Path.GetFileNameWithoutExtension(files[i]);
                var nameSplit = fileName.Split('@');
                if (nameSplit.Length <= 1)
                    continue;
                
                var createTimeTicks = new DateTime(long.Parse(nameSplit[1]));
                if (createTimeTicks < deleteTimeTicks)
                    File.Delete(files[i]);
            }
            
            var logMessage = _stringBuilder.ToString();
            if (string.IsNullOrEmpty(logMessage))
                return;
        
            var filePath = Path.Combine(directoryPath, $"{_nextWriteTime.AddMinutes(-writeTimeMinutes):yy-MM-dd_HH_mm_ss}~{_nextWriteTime:yy-MM-dd_HH_mm_ss}@{_nextWriteTime.Ticks}.log");
            File.WriteAllText(filePath, logMessage);

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