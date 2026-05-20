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
    public async Task StartCommandHandler_ShouldHandleStart()
    {
        var mockBot = new Mock<ITelegramBot>();
        var logger = new Mock<ILogger<StartCommandHandler>>();
        var handler = new StartCommandHandler(mockBot.Object, logger.Object);
        var message = new Message { Chat = new Chat { Id = 1 }, Text = "/start" };

        var result = await handler.HandleAsync(message, CancellationToken.None);
        
        Assert.True(result);
        mockBot.Verify(b => b.SendMessageAsync(1, It.Is<string>(s => s.Contains("Привет")), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HelpCommandHandler_ShouldHandleHelp()
    {
        var mockBot = new Mock<ITelegramBot>();
        var logger = new Mock<ILogger<HelpCommandHandler>>();
        var handler = new HelpCommandHandler(mockBot.Object, logger.Object);
        var message = new Message { Chat = new Chat { Id = 2 }, Text = "/help" };

        var result = await handler.HandleAsync(message, CancellationToken.None);
        
        Assert.True(result);
        mockBot.Verify(b => b.SendMessageAsync(2, It.Is<string>(s => s.Contains("Команды")), It.IsAny<CancellationToken>()), Times.Once);
    }
}