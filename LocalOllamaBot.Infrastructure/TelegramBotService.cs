using LocalOllamaBot.Core.Abstractions;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace LocalOllamaBot.Infrastructure;

public class TelegramBotService : ITelegramBot
{
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<TelegramBotService> _logger;
    private readonly IChatService _chatService;
    private readonly ICommandHandler _commandChain; // цепочка обработчиков команд

    public TelegramBotService(string token, IChatService chatService, ILogger<TelegramBotService> logger)
    {
        _botClient = new TelegramBotClient(token);
        _chatService = chatService;
        _logger = logger;
        
        // Строим цепочку: /start -> /help -> дальше другие команды
        var startHandler = new StartCommandHandler(this, logger);
        var helpHandler = new HelpCommandHandler(this, logger);
        startHandler.SetNext(helpHandler);
        _commandChain = startHandler; // начало цепочки
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Запуск Telegram бота...");
        _botClient.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            cancellationToken: cancellationToken
        );
        // Бесконечное ожидание
        await Task.Delay(Timeout.Infinite, cancellationToken);
    }

    private async Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken ct)
    {
        if (update.Message is not { } message || string.IsNullOrEmpty(message.Text))
            return;

        // Сначала пробуем обработать как команду 
        var handled = await _commandChain.HandleAsync(message, ct);
        if (handled) return; // команда обработана, выходим

        // Если не команда - отправляем в ллмку
        _logger.LogInformation("Получено сообщение от {ChatId}: {Text}", message.Chat.Id, message.Text);
        
        // Отправляет индикатор "печатает"
        await SendTypingAsync(message.Chat.Id, ct);
        
        // Получает ответ от нейросети
        var response = await _chatService.ProcessMessageAsync(message.Chat.Id, message.Text, ct);
        
        // Отправляет ответ пользователю
        await SendMessageAsync(message.Chat.Id, response, ct);
    }

    private async Task SendTypingAsync(long chatId, CancellationToken ct)
    {
        try
        {
            await _botClient.SendChatActionAsync(chatId, ChatAction.Typing, cancellationToken: ct);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Не удалось отправить индикатор печати (возможно, чат заблокирован)");
        }
    }

    private Task HandleErrorAsync(ITelegramBotClient client, Exception exception, CancellationToken ct)
    {
        _logger.LogError(exception, "Критическая ошибка Telegram бота");
        return Task.CompletedTask;
    }

    public async Task SendMessageAsync(long chatId, string text, CancellationToken cancellationToken)
    {
        await _botClient.SendTextMessageAsync(chatId, text, cancellationToken: cancellationToken);
    }
}