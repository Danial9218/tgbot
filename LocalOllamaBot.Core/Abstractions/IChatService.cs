namespace LocalOllamaBot.Core.Abstractions;

public interface IChatService
{
    Task<string> ProcessMessageAsync(long chatId, string userMessage, CancellationToken cancellationToken);
}