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
    public async Task SendMessageAsync_DoesNotThrow()
    {
        var mockChat = new Mock<IChatService>();
        var logger = Mock.Of<ILogger<TelegramBotService>>();
        var service = new TelegramBotService("fake", mockChat.Object, logger);

        var mockBotClient = new Mock<ITelegramBotClient>();
        var field = typeof(TelegramBotService).GetField("_botClient",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(service, mockBotClient.Object);

        await service.SendMessageAsync(123, "test", CancellationToken.None);
        // Без проверки, просто убеждаемся, что метод не падает
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
            var task = method?.Invoke(service, new object[] { Mock.Of<ITelegramBotClient>(), new Exception("error"), CancellationToken.None }) as Task;
            task?.GetAwaiter().GetResult();
        });
        Assert.Null(exception);
    }
}