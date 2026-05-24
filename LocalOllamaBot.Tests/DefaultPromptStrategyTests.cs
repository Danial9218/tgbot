using LocalOllamaBot.Application;
using Xunit;

namespace LocalOllamaBot.Tests;

public class DefaultPromptStrategyTests
{
    [Fact]
    public void BuildPrompt_ShouldNotBeEmpty()
    {
        var strategy = new DefaultPromptStrategy();
        var result = strategy.BuildPrompt("Привет");
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public void BuildPrompt_ShouldContainUserMessage()
    {
        var strategy = new DefaultPromptStrategy();
        var result = strategy.BuildPrompt("Тест123");
        Assert.Contains("Пользователь: Тест123", result);
    }

    [Fact]
    public void BuildPrompt_ShouldContainAssistantLabel()
    {
        var strategy = new DefaultPromptStrategy();
        var result = strategy.BuildPrompt("Любой текст");
        Assert.Contains("Ассистент:", result);
    }
}