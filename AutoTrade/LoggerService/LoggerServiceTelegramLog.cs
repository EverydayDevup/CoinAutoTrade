using Telegram.Bot;
using Telegram.Bot.Types;

namespace LoggerService;

/// <summary>
/// 텔레그램으로 보내는 로그를 관리
/// </summary>
internal class LoggerServiceTelegramLog(string name, string apiToken, long chatId)
{
    private TelegramBotClient BotClient { get; } = new (apiToken);
    private ChatId ChatId { get; } = new (chatId);

    private string Name { get; } = name;
    
    public async Task<Message> SendMessage(string message)
    {
        return await BotClient.SendMessage(ChatId, $"[{Name}] {message}");
    }
}