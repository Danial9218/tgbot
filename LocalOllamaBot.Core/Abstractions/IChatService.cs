// Интерфейс сервиса, который отвечает за обработку сообщений пользователя
namespace LocalOllamaBot.Core.Abstractions;

public interface IChatService
{
    // Принимает текст от пользователя, возвращает ответ от нейросети
    Task<string> ProcessMessageAsync(long chatId, string userMessage, CancellationToken cancellationToken);
}