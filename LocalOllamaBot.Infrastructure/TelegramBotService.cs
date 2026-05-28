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

        // КРАСИВЫЙ ВЫВОД В КОНСОЛЬ 
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write($"[{DateTime.Now:HH:mm:ss}] ");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("ПОЛЬЗОВАТЕЛЬ");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write($" [{message.Chat.Id}]: ");
        Console.ForegroundColor = ConsoleColor.Black;
        Console.WriteLine($"{message.Text}");
        Console.ResetColor();
        // 
        
        // Сначала обработать как команду 
        var handled = await _commandChain.HandleAsync(message, ct);
        if (handled) return;

        _logger.LogInformation("Получено сообщение от {ChatId}: {Text}", message.Chat.Id, message.Text);
    
        await SendTypingAsync(message.Chat.Id, ct);
    
        var response = await _chatService.ProcessMessageAsync(message.Chat.Id, message.Text, ct);
    
        // КРАСИВЫЙ ВЫВОД ОТВЕТА 
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write($"[{DateTime.Now:HH:mm:ss}] ");
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.Write("БОТ");
        Console.ForegroundColor = ConsoleColor.Black;
        Console.Write($" [{message.Chat.Id}]: ");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"{response}");
        Console.ResetColor();
        Console.WriteLine(); // пустая строка для разделения
        // 
        
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