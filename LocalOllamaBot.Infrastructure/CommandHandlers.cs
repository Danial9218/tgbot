using LocalOllamaBot.Core.Abstractions;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace LocalOllamaBot.Infrastructure;

// Интерфейс обработчика команды 
public interface ICommandHandler
{
    Task<bool> HandleAsync(Message message, CancellationToken ct);
    void SetNext(ICommandHandler next);
}

// Абстрактный базовый класс для всех обработчиков
public abstract class CommandHandlerBase : ICommandHandler
{
    private ICommandHandler? _nextHandler;

    public void SetNext(ICommandHandler next) => _nextHandler = next;

    public virtual async Task<bool> HandleAsync(Message message, CancellationToken ct)
    {
        // Если не обработали, передаём дальше
        if (_nextHandler != null)
            return await _nextHandler.HandleAsync(message, ct);
        return false;
    }
}

// Обработчик команды /start
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
        if (message.Text != null && message.Text.StartsWith("/start"))
        {
            _logger.LogInformation("Пользователь {ChatId} написал /start", message.Chat.Id);
            await _bot.SendMessageAsync(message.Chat.Id, 
                "Привет! Я бот на локальной нейросети Qwen3.5. Задавай любые вопросы, я отвечу.", 
                ct);
            return true; // команда обработана
        }
        return await base.HandleAsync(message, ct);
    }
}

// Обработчик команды /help
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
        if (message.Text != null && message.Text.StartsWith("/help"))
        {
            var helpText = "Доступные команды:\n" +
                           "/start – запустить бота\n" +
                           "/help – показать эту справку\n" +
                           "/stats – статистика сессии (позже добавлю)\n" +
                           "/clear – очистить историю диалога";
            await _bot.SendMessageAsync(message.Chat.Id, helpText, ct);
            return true;
        }
        return await base.HandleAsync(message, ct);
    }
    
}

