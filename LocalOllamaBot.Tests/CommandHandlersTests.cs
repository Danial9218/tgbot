using LocalOllamaBot.Core.Abstractions;
using LocalOllamaBot.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;
using Telegram.Bot.Types;
using Xunit;

namespace LocalOllamaBot.Tests;

public class CommandHandlersTests
{
    [Fact]
    public async Task StartCommandHandler_ShouldRespondOnStart()
    {
        var mockBot = new Mock<ITelegramBot>();
        var logger = Mock.Of<ILogger>();
        var handler = new StartCommandHandler(mockBot.Object, logger);
        var message = new Message { Chat = new Chat { Id = 1 }, Text = "/start" };

        var result = await handler.HandleAsync(message, CancellationToken.None);

        Assert.True(result);
        mockBot.Verify(b => b.SendMessageAsync(1, It.Is<string>(s => s.Contains("Привет")), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HelpCommandHandler_ShouldRespondOnHelp()
    {
        var mockBot = new Mock<ITelegramBot>();
        var logger = Mock.Of<ILogger>();
        var handler = new HelpCommandHandler(mockBot.Object, logger);
        var message = new Message { Chat = new Chat { Id = 1 }, Text = "/help" };

        var result = await handler.HandleAsync(message, CancellationToken.None);

        Assert.True(result);
        mockBot.Verify(b => b.SendMessageAsync(1, It.Is<string>(s => s.Contains("команды")), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task Chain_CallsNextHandlerWhenCurrentCannotHandle()
    {
        var mockBot = new Mock<ITelegramBot>();
        var logger = Mock.Of<ILogger>();
        var nextMock = new Mock<ICommandHandler>();
        nextMock.Setup(x => x.HandleAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var startHandler = new StartCommandHandler(mockBot.Object, logger);
        startHandler.SetNext(nextMock.Object);

        var unknownMsg = new Message { Chat = new Chat { Id = 1 }, Text = "/unknown" };
        var result = await startHandler.HandleAsync(unknownMsg, CancellationToken.None);

        Assert.True(result);
        nextMock.Verify(x => x.HandleAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()), Times.Once);
        mockBot.Verify(b => b.SendMessageAsync(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task StartCommandHandler_WithNullText_ShouldReturnFalse()
    {
        var mockBot = new Mock<ITelegramBot>();
        var handler = new StartCommandHandler(mockBot.Object, Mock.Of<ILogger>());

        var message = new Message
        {
            Chat = new Chat { Id = 1 },
            Text = null
        };

        var result = await handler.HandleAsync(message, CancellationToken.None);
        Assert.False(result);
    }

    [Fact]
    public async Task StartCommandHandler_WithEmptyText_ShouldReturnFalse()
    {
        var mockBot = new Mock<ITelegramBot>();
        var handler = new StartCommandHandler(mockBot.Object, Mock.Of<ILogger>());

        var message = new Message
        {
            Chat = new Chat { Id = 1 },
            Text = ""
        };

        var result = await handler.HandleAsync(message, CancellationToken.None);
        Assert.False(result);
    }

    [Fact]
    public async Task HelpCommandHandler_WithUnknownCommand_ShouldReturnFalse()
    {
        var mockBot = new Mock<ITelegramBot>();
        var handler = new HelpCommandHandler(mockBot.Object, Mock.Of<ILogger>());

        var message = new Message
        {
            Chat = new Chat { Id = 1 },
            Text = "/unknown"
        };

        var result = await handler.HandleAsync(message, CancellationToken.None);
        Assert.False(result);
    }

    [Fact]
    public async Task HelpCommandHandler_WithNullText_ShouldReturnFalse()
    {
        var mockBot = new Mock<ITelegramBot>();
        var handler = new HelpCommandHandler(mockBot.Object, Mock.Of<ILogger>());

        var message = new Message
        {
            Chat = new Chat { Id = 1 },
            Text = null
        };

        var result = await handler.HandleAsync(message, CancellationToken.None);
        Assert.False(result);
    }

    [Fact]
    public async Task HelpCommandHandler_WithEmptyText_ShouldReturnFalse()
    {
        var mockBot = new Mock<ITelegramBot>();
        var handler = new HelpCommandHandler(mockBot.Object, Mock.Of<ILogger>());

        var message = new Message
        {
            Chat = new Chat { Id = 1 },
            Text = ""
        };

        var result = await handler.HandleAsync(message, CancellationToken.None);
        Assert.False(result);
    }

    [Fact]
    public async Task StartCommandHandler_ShouldNotCallNextHandler_WhenHandled()
    {
        var mockBot = new Mock<ITelegramBot>();
        var next = new Mock<ICommandHandler>();
        var handler = new StartCommandHandler(mockBot.Object, Mock.Of<ILogger>());
        handler.SetNext(next.Object);

        var message = new Message
        {
            Chat = new Chat { Id = 1 },
            Text = "/start"
        };

        var result = await handler.HandleAsync(message, CancellationToken.None);

        Assert.True(result);
        next.Verify(x => x.HandleAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task HelpCommandHandler_ShouldNotCallNextHandler_WhenHandled()
    {
        var mockBot = new Mock<ITelegramBot>();
        var next = new Mock<ICommandHandler>();
        var handler = new HelpCommandHandler(mockBot.Object, Mock.Of<ILogger>());
        handler.SetNext(next.Object);

        var message = new Message
        {
            Chat = new Chat { Id = 1 },
            Text = "/help"
        };

        var result = await handler.HandleAsync(message, CancellationToken.None);

        Assert.True(result);
        next.Verify(x => x.HandleAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task HelpCommandHandler_ShouldCallNext_WhenNotHelp()
    {
        var mockBot = new Mock<ITelegramBot>();
        var next = new Mock<ICommandHandler>();
        next.Setup(x => x.HandleAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new HelpCommandHandler(mockBot.Object, Mock.Of<ILogger>());
        handler.SetNext(next.Object);

        var message = new Message
        {
            Chat = new Chat { Id = 1 },
            Text = "/start"
        };

        var result = await handler.HandleAsync(message, CancellationToken.None);

        Assert.True(result);
        next.Verify(x => x.HandleAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()), Times.Once);
        mockBot.Verify(b => b.SendMessageAsync(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task StartCommandHandler_ShouldCallNext_WhenNotStart()
    {
        var mockBot = new Mock<ITelegramBot>();
        var next = new Mock<ICommandHandler>();
        next.Setup(x => x.HandleAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new StartCommandHandler(mockBot.Object, Mock.Of<ILogger>());
        handler.SetNext(next.Object);

        var message = new Message
        {
            Chat = new Chat { Id = 1 },
            Text = "/help"
        };

        var result = await handler.HandleAsync(message, CancellationToken.None);

        Assert.True(result);
        next.Verify(x => x.HandleAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()), Times.Once);
        mockBot.Verify(b => b.SendMessageAsync(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}