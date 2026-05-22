namespace LocalOllamaBot.Core.Abstractions;

public interface ITelegramBot
{
    // Запустить бота (начать приём сообщений)
    Task StartAsync(CancellationToken cancellationToken);
    
    // Отправить сообщение в чат
    Task SendMessageAsync(long chatId, string text, CancellationToken cancellationToken);
}