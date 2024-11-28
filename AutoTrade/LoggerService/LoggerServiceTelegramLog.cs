using Telegram.Bot;
using Telegram.Bot.Types;

namespace LoggerService;

internal class LoggerServiceTelegramLog(string apiToken, long chatId)
{
    private TelegramBotClient BotClient { get; } = new (apiToken);
    private ChatId ChatId { get; } = new (chatId);
    
    public async Task<Message> SendMessage(string message)
    {
        return await BotClient.SendMessage(ChatId, message);
    }
}