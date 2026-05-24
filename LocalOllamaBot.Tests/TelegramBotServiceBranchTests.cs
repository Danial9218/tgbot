using LocalOllamaBot.Core.Abstractions;
using LocalOllamaBot.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;
using Telegram.Bot.Types;
using Xunit;

namespace LocalOllamaBot.Tests;

public class TelegramBotServiceBranchTests
{
    private static async Task InvokeHandleUpdateAsync(
        TelegramBotService service,
        Update update)
    {
        var method = typeof(TelegramBotService)
            .GetMethod(
                "HandleUpdateAsync",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

        Assert.NotNull(method);

        var task = method!.Invoke(
            service,
            new object[]
            {
                null!,
                update,
                CancellationToken.None
            }) as Task;

        if (task != null)
            await task;
    }

    [Fact]
    public async Task HandleUpdateAsync_WithNullMessage_ShouldReturn()
    {
        var mockChat = new Mock<IChatService>();

        var service = new TelegramBotService(
            "123456:TEST_TOKEN",
            mockChat.Object,
            Mock.Of<ILogger<TelegramBotService>>());

        var update = new Update
        {
            Message = null
        };

        var exception = await Record.ExceptionAsync(async () =>
        {
            await InvokeHandleUpdateAsync(service, update);
        });

        Assert.Null(exception);
    }

    [Fact]
    public async Task HandleUpdateAsync_WithEmptyText_ShouldReturn()
    {
        var mockChat = new Mock<IChatService>();

        var service = new TelegramBotService(
            "123456:TEST_TOKEN",
            mockChat.Object,
            Mock.Of<ILogger<TelegramBotService>>());

        var update = new Update
        {
            Message = new Message
            {
                Text = ""
            }
        };

        var exception = await Record.ExceptionAsync(async () =>
        {
            await InvokeHandleUpdateAsync(service, update);
        });

        Assert.Null(exception);

        mockChat.Verify(
            x => x.ProcessMessageAsync(
                It.IsAny<long>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

  

    [Fact]
    public async Task HandleUpdateAsync_WithRegularMessage_ShouldCallChatService()
    {
        var mockChat = new Mock<IChatService>();

        mockChat.Setup(x =>
                x.ProcessMessageAsync(
                    It.IsAny<long>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync("response");

        var service = new TelegramBotService(
            "123456:TEST_TOKEN",
            mockChat.Object,
            Mock.Of<ILogger<TelegramBotService>>());

        var update = new Update
        {
            Message = new Message
            {
                Chat = new Chat
                {
                    Id = 1
                },
                Text = "hello"
            }
        };

        var exception = await Record.ExceptionAsync(async () =>
        {
            await InvokeHandleUpdateAsync(service, update);
        });

        Assert.NotNull(exception);

        mockChat.Verify(
            x => x.ProcessMessageAsync(
                1,
                "hello",
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}