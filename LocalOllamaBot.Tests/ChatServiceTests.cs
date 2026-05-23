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
        // Подготовка (Arrange)
        var mockOllama = new Mock<IOllamaService>();
        mockOllama.Setup(o => o.IsModelAvailableAsync(It.IsAny<CancellationToken>()))
                  .ReturnsAsync(true);
        mockOllama.Setup(o => o.GenerateResponseAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync("Ответ от нейросети");

        var mockLogger = new Mock<ILogger<ChatService>>();
        var mockStrategy = new Mock<IPromptStrategy>();
        mockStrategy.Setup(s => s.BuildPrompt(It.IsAny<string>(), It.IsAny<List<string>>()))
                    .Returns("test prompt");

        var service = new ChatService(mockOllama.Object, mockLogger.Object, mockStrategy.Object);

        // Действие (Act)
        var result = await service.ProcessMessageAsync(123, "Привет", CancellationToken.None);

        // Проверка (Assert)
        Assert.Equal("Ответ от нейросети", result);
        mockOllama.Verify(o => o.GenerateResponseAsync("test prompt", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ProcessMessageAsync_WhenOllamaUnavailable_ReturnsErrorMessage()
    {
        var mockOllama = new Mock<IOllamaService>();
        mockOllama.Setup(o => o.IsModelAvailableAsync(It.IsAny<CancellationToken>()))
                  .ReturnsAsync(false);
        var mockLogger = new Mock<ILogger<ChatService>>();
        var mockStrategy = new Mock<IPromptStrategy>();

        var service = new ChatService(mockOllama.Object, mockLogger.Object, mockStrategy.Object);

        var result = await service.ProcessMessageAsync(123, "Привет", CancellationToken.None);

        Assert.Contains("Ollama не запущен", result);
    }
}

    