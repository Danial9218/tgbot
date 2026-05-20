using LocalOllamaBot.Application;
using LocalOllamaBot.Core.Abstractions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LocalOllamaBot.Tests;

public class ChatServiceTests
{
    [Fact]
    public async Task ProcessMessageAsync_WhenOllamaAvailable_ReturnsResponse()
    {
        var mockOllama = new Mock<IOllamaService>();
        mockOllama.Setup(o => o.IsModelAvailableAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);
        mockOllama.Setup(o => o.GenerateResponseAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("Привет, это тест");
        var mockLogger = new Mock<ILogger<ChatService>>();
        var service = new ChatService(mockOllama.Object, mockLogger.Object);

        var result = await service.ProcessMessageAsync(123, "Тест", CancellationToken.None);
        
        Assert.Equal("Привет, это тест", result);
        mockOllama.Verify(o => o.GenerateResponseAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ProcessMessageAsync_WhenOllamaUnavailable_ReturnsErrorMessage()
    {
        var mockOllama = new Mock<IOllamaService>();
        mockOllama.Setup(o => o.IsModelAvailableAsync(It.IsAny<CancellationToken>())).ReturnsAsync(false);
        var mockLogger = new Mock<ILogger<ChatService>>();
        var service = new ChatService(mockOllama.Object, mockLogger.Object);

        var result = await service.ProcessMessageAsync(123, "Тест", CancellationToken.None);
        
        Assert.Contains("Ollama не запущен", result);
    }
}