namespace LocalOllamaBot.Core.Abstractions;

public interface IOllamaService
{
    // Отправить запрос к Ollama и получить ответ
    Task<string> GenerateResponseAsync(string prompt, CancellationToken cancellationToken);
    
    // Проверить, доступен ли Ollama 
    Task<bool> IsModelAvailableAsync(CancellationToken cancellationToken);
}