using LocalOllamaBot.Core.Abstractions;
using LocalOllamaBot.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LocalOllamaBot.Tests;

public class TelegramBotServiceCoverageTests
{
    [Fact]
    public void Constructor_ShouldCreateService()
    {
        var chat = new Mock<IChatService>();

        var logger = Mock.Of<ILogger<TelegramBotService>>();

        var service = new TelegramBotService(
            "fake",
            chat.Object,
            logger);

        Assert.NotNull(service);
    }

    [Fact]
    public void PrivateFields_ShouldBeInitialized()
    {
        var chat = new Mock<IChatService>();

        var logger = Mock.Of<ILogger<TelegramBotService>>();

        var service = new TelegramBotService(
            "fake",
            chat.Object,
            logger);

        var botField = typeof(TelegramBotService)
            .GetField(
                "_botClient",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

        var chatField = typeof(TelegramBotService)
            .GetField(
                "_chatService",
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

        var service = new TelegramBotService(
            "fake",
            chat.Object,
            logger);

        var method = typeof(TelegramBotService)
            .GetMethod(
                "HandleErrorAsync",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

        var exception = Record.Exception(() =>
        {
            var task = method?.Invoke(
                service,
                new object[]
                {
                    Mock.Of<Telegram.Bot.ITelegramBotClient>(),
                    new Exception("test"),
                    CancellationToken.None
                }) as Task;

            task?.GetAwaiter().GetResult();
        });

        Assert.Null(exception);
    }
}