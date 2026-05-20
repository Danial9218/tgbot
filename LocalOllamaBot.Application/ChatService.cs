using LocalOllamaBot.Core.Abstractions;
using Microsoft.Extensions.Logging;

namespace LocalOllamaBot.Application;

public class ChatService : IChatService
{
    private readonly IOllamaService _ollamaService;
    private readonly ILogger<ChatService> _logger;
    private readonly IPromptStrategy _promptStrategy;
    private readonly UserSessionManager _sessionManager;

    public ChatService(IOllamaService ollamaService, ILogger<ChatService> logger, IPromptStrategy? strategy = null)
    {
        _ollamaService = ollamaService;
        _logger = logger;
        _promptStrategy = strategy ?? new DefaultPromptStrategy();
        _sessionManager = UserSessionManager.Instance;
    }

    public async Task<string> ProcessMessageAsync(long chatId, string userMessage, CancellationToken cancellationToken)
    {
        if (!await _ollamaService.IsModelAvailableAsync(cancellationToken))
            return " Ollama не запущен. Пожалуйста, запустите `ollama serve`.";

        var history = _sessionManager.GetHistory(chatId);
        var prompt = _promptStrategy.BuildPrompt(userMessage, history);

        _sessionManager.AddMessage(chatId, "User: " + userMessage);
        var response = await _ollamaService.GenerateResponseAsync(prompt, cancellationToken);
        _sessionManager.AddMessage(chatId, "Bot: " + response);

        return response;
    }
}