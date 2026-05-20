namespace LocalOllamaBot.Core.Abstractions;

public interface ITelegramBot
{
    Task StartAsync(CancellationToken cancellationToken);
    Task SendMessageAsync(long chatId, string text, CancellationToken cancellationToken);
}