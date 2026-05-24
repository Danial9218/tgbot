using LocalOllamaBot.Core.Abstractions;
using LocalOllamaBot.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;
using Telegram.Bot;
using Xunit;

namespace LocalOllamaBot.Tests;

public class TelegramBotServiceCoverageTests
{
    [Fact]
    public void Constructor_ShouldCreateService()
    {
        var chat = new Mock<IChatService>();
        var logger = Mock.Of<ILogger<TelegramBotService>>();
        var service = new TelegramBotService("fake", chat.Object, logger);
        Assert.NotNull(service);
    }

    [Fact]
    public void PrivateFields_ShouldBeInitialized()
    {
        var chat = new Mock<IChatService>();
        var logger = Mock.Of<ILogger<TelegramBotService>>();
        var service = new TelegramBotService("fake", chat.Object, logger);

        var botField = typeof(TelegramBotService)
            .GetField("_botClient",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

        var chatField = typeof(TelegramBotService)
            .GetField("_chatService",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

        Assert.NotNull(botField?.GetValue(service));
        Assert.NotNull(chatField?.GetValue(service));
    }

    [Fact]
    public void HandleErrorAsync_ShouldNotThrow()
    {
        var chat = new Mock<IChatService>();
        var logger = Mock.Of<ILogger<TelegramBotService>>();
        var service = new TelegramBotService("fake", chat.Object, logger);

        var method = typeof(TelegramBotService)
            .GetMethod("HandleErrorAsync",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

        var exception = Record.Exception(() =>
        {
            var task = method?.Invoke(service, new object[]
            {
                Mock.Of<ITelegramBotClient>(),
                new Exception("test"),
                CancellationToken.None
            }) as Task;

            task?.GetAwaiter().GetResult();
        });

        Assert.Null(exception);
    }

    [Fact]
    public async Task SendTypingAsync_ShouldNotThrow()
    {
        var mockChat = new Mock<IChatService>();
        var logger = Mock.Of<ILogger<TelegramBotService>>();
        var service = new TelegramBotService("fake", mockChat.Object, logger);

        var mockBotClient = new Mock<ITelegramBotClient>();
        var field = typeof(TelegramBotService).GetField("_botClient",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(service, mockBotClient.Object);

        var method = typeof(TelegramBotService).GetMethod("SendTypingAsync",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var exception = await Record.ExceptionAsync(async () =>
        {
            var task = method?.Invoke(service, new object[] { 123L, CancellationToken.None }) as Task;
            if (task != null) await task;
        });

        Assert.Null(exception);
    }

    [Fact]
    public async Task SendMessageAsync_ShouldNotThrow()
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
            await service.SendMessageAsync(123, "test message", CancellationToken.None);
        });

        Assert.Null(exception);
    }
}