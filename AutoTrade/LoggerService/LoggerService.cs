﻿using System.Diagnostics;

namespace LoggerService;

/// <summary>
/// 로그와 관련 로직을 처리하는 클래스
/// </summary>
public class LoggerService : IDisposable
{
    private readonly LoggerServiceTelegramLog? _telegramLog;
    private readonly LoggerServiceConsole _loggerServiceConsole = new();
    private readonly Dictionary<string, LoggerServiceFileLog> _dicFileLogs = new();
    private readonly int _fileLogWriteTimeMinutes = 1;
    private bool _isDisposed = false;

    public LoggerService()
    {
        _telegramLog = null;
        Update();
    }

    public LoggerService(string telegramApiToken, long telegramChatId)
    {
        _telegramLog = new LoggerServiceTelegramLog(telegramApiToken, telegramChatId);
        Update();
    }

    private async void Update()
    {
        while (true)
        {
            await Task.Delay(_fileLogWriteTimeMinutes * 60 * 1000);
            
            foreach (var (_, fileLog) in _dicFileLogs)
                fileLog.Save();
        }
        // ReSharper disable once FunctionNeverReturns
    }
    
    /// <summary>
    /// 텔레그램에 로그를 남김
    /// </summary>
    public async Task TelegramAsync(string message)
    {
        if (_telegramLog == null)
            return;
        
        await _telegramLog.SendMessage(message);
    }

    /// <summary>
    /// 파일에 로그를 남김
    /// </summary>
    public void FileLog(string directoryPath, string message)
    {
        if (!_dicFileLogs.TryGetValue(directoryPath, out var file))
        {
            file = new LoggerServiceFileLog(directoryPath, _fileLogWriteTimeMinutes);
            _dicFileLogs.Add(directoryPath, file);
        }
        
        file.Log(message);
    }

    /// <summary>
    /// 콘솔에 로그를 남김
    /// </summary>
    public void ConsoleLog(string message)
    {
        _loggerServiceConsole.Log(message);
    }
    
    /// <summary>
    /// 콘솔에 에러를 남김
    /// </summary>
    public void ConsoleError(string message)
    {
        _loggerServiceConsole.Error(message);
    }

    /// <summary>
    /// 콘솔에 디버그 로그를 남김
    /// </summary>
    [Conditional("DEBUG")]
    public void Debug(string message)
    {
        _loggerServiceConsole.Debug(message);
    }
    
    public void Dispose()
    {
        if (_isDisposed)
            return;
        
        _isDisposed = false;
        
        foreach (var (_, fileLog) in _dicFileLogs)
            fileLog.Save();
        
        GC.SuppressFinalize(this);
    }

    ~LoggerService()
    {
        Dispose();
    }
}