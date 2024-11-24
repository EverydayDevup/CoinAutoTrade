namespace AutoTrade.Logic;

/// <summary>
/// 트레이드 봇 내부에서 사용할 메시지 관련 처리 담당
/// </summary>
public static class MessageManager
{
    private enum EMessageType
    {
        Debug = 0, // 디버그 빌드에만 표시할 콘솔 메시지
        Info, // 릴리즈 빌드에 표시할 콘솔 메시지
        Notify, // 텔레그램과 콘솔에 표시할 메시지
        Error, // 텔레그램과 콘솔에 표시할 에러 메시지 
    }
    
    /// <summary>
    /// 디버그 빌드에서만 콘솔에 표시할 메시지
    /// </summary>
    public static void Log(string message)
    {
        Notify(EMessageType.Debug, message);
    }
    
    /// <summary>
    /// 릴리즈 빌드에서도 콘솔에 표시할 메시지
    /// </summary>
    public static void Info(string message)
    {
        Notify(EMessageType.Info, message);
    }
    
    /// <summary>
    /// 텔레그램과 콘솔에 표시할 메시지
    /// </summary>
    public static void Notify(string message)
    {
        Notify(EMessageType.Notify, message);
    }

    /// <summary>
    /// 텔레그램과 콘솔에 표시할 에러 메시지
    /// </summary>
    public static void Error(string message)
    {
        Notify(EMessageType.Error, message);
    }

    private static void Notify(EMessageType messageType, string message)
    {
        var msg = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}][{messageType.ToString().ToUpper()}] {message}";
        
        #if !DEBUG
        if (notifyType == ENotifyType.Debug)
            return;
        #endif
        
        Console.WriteLine(msg);
        
        if (messageType == EMessageType.Notify || 
            messageType == EMessageType.Error)
            TelegramBot.Send(msg);
    }

    public static string GetLine()
    {
       return "=============================================";
    }
}