using LocalOllamaBot.Core.Abstractions;
using Microsoft.Extensions.Logging;

namespace LocalOllamaBot.Application;

// Реализует обработку сообщений: берёт историю, строит промпт, получает ответ от Ollama
public class ChatService : IChatService
{
    private readonly IOllamaService _ollama;
    private readonly ILogger<ChatService> _logger;
    private readonly IPromptStrategy _strategy;
    private readonly UserSessionManager _sessionManager;

    // Внедрение зависимостей через конструктор
    public ChatService(IOllamaService ollama, ILogger<ChatService> logger, IPromptStrategy strategy)
    {
        _ollama = ollama;
        _logger = logger;
        _strategy = strategy;
        _sessionManager = UserSessionManager.Instance; // получаем синглтон
    }

    public async Task<string> ProcessMessageAsync(long chatId, string userMessage, CancellationToken cancellationToken)
    {
        // Проверяем, доступна ли модель
        if (!await _ollama.IsModelAvailableAsync(cancellationToken))
        {
            _logger.LogWarning("Ollama не доступен! Проверьте, запущен ли сервер.");
            return "Ошибка: Ollama не запущен. Пожалуйста, запустите `ollama serve`.";
        }

        // Загружаем историю чата (если есть)
        var history = _sessionManager.GetHistory(chatId);
        // Строим промпт с помощью выбранной стратегии
        var prompt = _strategy.BuildPrompt(userMessage, history);
        
        _logger.LogInformation("Отправляем запрос в Ollama для чата {ChatId}", chatId);
        
        // Сохраняем сообщение пользователя в историю
        _sessionManager.AddMessage(chatId, "Пользователь: " + userMessage);
        
        // Получаем ответ от нейросети
        var response = await _ollama.GenerateResponseAsync(prompt, cancellationToken);
        
        // Сохраняем ответ бота в историю
        _sessionManager.AddMessage(chatId, "Бот: " + response);
        
        return response;
    }
}