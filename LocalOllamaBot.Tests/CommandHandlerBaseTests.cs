using LocalOllamaBot.Infrastructure;
using Moq;
using Telegram.Bot.Types;
using Xunit;

namespace LocalOllamaBot.Tests;

public class CommandHandlerBaseTests
{
    [Fact]
    public async Task HandleAsync_WhenNoNextHandler_ReturnsFalse()
    {
        var handler = new TestCommandHandler();
        var result = await handler.HandleAsync(new Message(), CancellationToken.None);
        Assert.False(result);
    }

    [Fact]
    public async Task HandleAsync_WhenNextHandlerExists_CallsIt()
    {
        var mockNext = new Mock<ICommandHandler>();
        mockNext.Setup(x => x.HandleAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new TestCommandHandler();
        handler.SetNext(mockNext.Object);

        var result = await handler.HandleAsync(new Message(), CancellationToken.None);
        Assert.True(result);
        mockNext.Verify(x => x.HandleAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    private class TestCommandHandler : CommandHandlerBase { }
}