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
    private readonly ICommandHandler _commandChain;

    // НОВЫЙ КОНСТРУКТОР с передачей HttpClient (уже настроенного на прокси)
    public TelegramBotService(string token, HttpClient httpClient, IChatService chatService, ILogger<TelegramBotService> logger)
    {
        _botClient = new TelegramBotClient(token, httpClient);
        _chatService = chatService;
        _logger = logger;

        // Цепочка обязанностей (без изменений)
        var startHandler = new StartCommandHandler(this, logger);
        var helpHandler = new HelpCommandHandler(this, logger);
        startHandler.SetNext(helpHandler);
        _commandChain = startHandler;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _botClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, cancellationToken: cancellationToken);
        await Task.Delay(Timeout.Infinite, cancellationToken);
    }

    private async Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken ct)
    {
        if (update.Message is not { } message || string.IsNullOrEmpty(message.Text))
            return;

        var handled = await _commandChain.HandleAsync(message, ct);
        if (handled) return;

        _logger.LogInformation("Сообщение от {ChatId}: {Text}", message.Chat.Id, message.Text);
        await SendTypingAsync(message.Chat.Id, ct);
        var response = await _chatService.ProcessMessageAsync(message.Chat.Id, message.Text, ct);
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
            _logger.LogWarning(ex, "Не удалось отправить индикатор печати");
        }
    }

    private Task HandleErrorAsync(ITelegramBotClient client, Exception exception, CancellationToken ct)
    {
        _logger.LogError(exception, "Ошибка Telegram бота");
        return Task.CompletedTask;
    }

    public async Task SendMessageAsync(long chatId, string text, CancellationToken cancellationToken)
    {
        await _botClient.SendTextMessageAsync(chatId, text, cancellationToken: cancellationToken);
    }
}