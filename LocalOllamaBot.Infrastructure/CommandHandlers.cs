using LocalOllamaBot.Core.Abstractions;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace LocalOllamaBot.Infrastructure;

public interface ICommandHandler
{
    Task<bool> HandleAsync(Message message, CancellationToken ct);
    void SetNext(ICommandHandler next);
}

public abstract class CommandHandlerBase : ICommandHandler
{
    private ICommandHandler? _next;

    public void SetNext(ICommandHandler next) => _next = next;

    public virtual async Task<bool> HandleAsync(Message message, CancellationToken ct)
    {
        return _next != null && await _next.HandleAsync(message, ct);
    }
}

public class StartCommandHandler : CommandHandlerBase
{
    private readonly ITelegramBot _bot;
    private readonly ILogger _logger;

    public StartCommandHandler(ITelegramBot bot, ILogger logger)
    {
        _bot = bot;
        _logger = logger;
    }

    public override async Task<bool> HandleAsync(Message message, CancellationToken ct)
    {
        if (message.Text?.StartsWith("/start") == true)
        {
            _logger.LogInformation("Обработка /start от {ChatId}", message.Chat.Id);
            await _bot.SendMessageAsync(message.Chat.Id, "🤖 Привет! Я бот на Qwen3.5 через Ollama. Задай любой вопрос.", ct);
            return true;
        }
        return await base.HandleAsync(message, ct);
    }
}

public class HelpCommandHandler : CommandHandlerBase
{
    private readonly ITelegramBot _bot;
    private readonly ILogger _logger;

    public HelpCommandHandler(ITelegramBot bot, ILogger logger)
    {
        _bot = bot;
        _logger = logger;
    }

    public override async Task<bool> HandleAsync(Message message, CancellationToken ct)
    {
        if (message.Text?.StartsWith("/help") == true)
        {
            await _bot.SendMessageAsync(message.Chat.Id, "📖 Команды:\n/start - приветствие\n/help - помощь\n/stats - статистика\n/clear - очистить историю", ct);
            return true;
        }
        return await base.HandleAsync(message, ct);
    }
}