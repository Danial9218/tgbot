using System.Net;
using LocalOllamaBot.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Xunit;

namespace LocalOllamaBot.Tests;

public class OllamaServiceTests
{
    [Fact]
    public void Constructor_DoesNotThrow()
    {
        var httpClient = new HttpClient();
        var logger = Mock.Of<ILogger<OllamaService>>();
        var service = new OllamaService(httpClient, logger);
        Assert.NotNull(service);
    }

    [Fact]
    public async Task IsModelAvailableAsync_WhenOllamaReturnsOk_ReturnsTrue()
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        var httpClient = new HttpClient(handlerMock.Object);
        var logger = Mock.Of<ILogger<OllamaService>>();
        var service = new OllamaService(httpClient, logger);

        var result = await service.IsModelAvailableAsync(CancellationToken.None);
        Assert.True(result);
    }

    [Fact]
    public async Task IsModelAvailableAsync_WhenOllamaReturnsError_ReturnsFalse()
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError));

        var httpClient = new HttpClient(handlerMock.Object);
        var logger = Mock.Of<ILogger<OllamaService>>();
        var service = new OllamaService(httpClient, logger);

        var result = await service.IsModelAvailableAsync(CancellationToken.None);
        Assert.False(result);
    }

    [Fact]
    public async Task GenerateResponseAsync_WhenValidResponse_ReturnsText()
    {
        var json = "{\"response\":\"Привет, мир!\"}";
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json)
            });

        var httpClient = new HttpClient(handlerMock.Object);
        var logger = Mock.Of<ILogger<OllamaService>>();
        var service = new OllamaService(httpClient, logger);

        var result = await service.GenerateResponseAsync("test", CancellationToken.None);
        Assert.Equal("Привет, мир!", result);
    }

    [Fact]
    public async Task GenerateResponseAsync_WhenHttpError_ReturnsErrorMessage()
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.ServiceUnavailable));

        var httpClient = new HttpClient(handlerMock.Object);
        var logger = Mock.Of<ILogger<OllamaService>>();
        var service = new OllamaService(httpClient, logger);

        var result = await service.GenerateResponseAsync("test", CancellationToken.None);
        Assert.Contains("Ошибка", result);
    }
}