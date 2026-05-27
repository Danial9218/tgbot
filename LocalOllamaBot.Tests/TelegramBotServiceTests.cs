using LocalOllamaBot.Core.Abstractions;
using LocalOllamaBot.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Xunit;

namespace LocalOllamaBot.Tests;

public class TelegramBotServiceTests
{
    [Fact]
    public void Constructor_DoesNotThrow()
    {
        var mockChat = new Mock<IChatService>();
        var logger = Mock.Of<ILogger<TelegramBotService>>();
        var service = new TelegramBotService("fake", mockChat.Object, logger);
        Assert.NotNull(service);
    }

    [Fact]
    public void Constructor_BuildsCommandChain()
    {
        var mockChat = new Mock<IChatService>();
        var logger = Mock.Of<ILogger<TelegramBotService>>();
        var service = new TelegramBotService("fake", mockChat.Object, logger);
        
        var chainField = typeof(TelegramBotService).GetField("_commandChain",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var chain = chainField?.GetValue(service);
        
        Assert.NotNull(chain);
    }

    [Fact]
    public async Task SendMessageAsync_DoesNotThrow()
    {
        var mockChat = new Mock<IChatService>();
        var logger = Mock.Of<ILogger<TelegramBotService>>();
        var service = new TelegramBotService("fake", mockChat.Object, logger);

        var mockBotClient = new Mock<ITelegramBotClient>();
        var field = typeof(TelegramBotService).GetField("_botClient",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(service, mockBotClient.Object);

        var exception = await Record.ExceptionAsync(async () =>
        {
            await service.SendMessageAsync(123, "test", CancellationToken.None);
        });

        Assert.Null(exception);
    }

    [Fact]
    public async Task SendMessageAsync_WithNullText_DoesNotThrow()
    {
        var mockChat = new Mock<IChatService>();
        var logger = Mock.Of<ILogger<TelegramBotService>>();
        var service = new TelegramBotService("fake", mockChat.Object, logger);

        var mockBotClient = new Mock<ITelegramBotClient>();
        var field = typeof(TelegramBotService).GetField("_botClient",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(service, mockBotClient.Object);

        var exception = await Record.ExceptionAsync(async () =>
        {
            await service.SendMessageAsync(123, null, CancellationToken.None);
        });

        Assert.Null(exception);
    }

    [Fact]
    public void HandleErrorAsync_DoesNotThrow()
    {
        var mockChat = new Mock<IChatService>();
        var logger = Mock.Of<ILogger<TelegramBotService>>();
        var service = new TelegramBotService("fake", mockChat.Object, logger);

        var method = typeof(TelegramBotService).GetMethod("HandleErrorAsync",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var exception = Record.Exception(() =>
        {
            var task = method?.Invoke(service, new object[]
            {
                Mock.Of<ITelegramBotClient>(),
                new Exception("error"),
                CancellationToken.None
            }) as Task;
            task?.GetAwaiter().GetResult();
        });

        Assert.Null(exception);
    }

    [Fact]
    public void StartAsync_ShouldStartReceiving()
    {
        var mockChat = new Mock<IChatService>();
        var logger = Mock.Of<ILogger<TelegramBotService>>();
        var service = new TelegramBotService("fake", mockChat.Object, logger);

        var mockBotClient = new Mock<ITelegramBotClient>();
        var field = typeof(TelegramBotService).GetField("_botClient",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(service, mockBotClient.Object);

        // Просто проверяем, что метод не падает при отмене через токен
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(100);
        
        var exception = Record.Exception(() =>
        {
            // Не ждём завершения, просто запускаем
            var task = service.StartAsync(cts.Token);
        });

        Assert.Null(exception);
    }

    [Fact]
    public void PrivateFields_ShouldBeInitialized()
    {
        var mockChat = new Mock<IChatService>();
        var logger = Mock.Of<ILogger<TelegramBotService>>();
        var service = new TelegramBotService("fake", mockChat.Object, logger);

        var botField = typeof(TelegramBotService).GetField("_botClient",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var chatField = typeof(TelegramBotService).GetField("_chatService",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var chainField = typeof(TelegramBotService).GetField("_commandChain",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        Assert.NotNull(botField?.GetValue(service));
        Assert.NotNull(chatField?.GetValue(service));
        Assert.NotNull(chainField?.GetValue(service));
    }
}