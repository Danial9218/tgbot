using LocalOllamaBot.Application;
using Xunit;

namespace LocalOllamaBot.Tests;

public class DefaultPromptStrategyTests
{
    [Fact]
    public void BuildPrompt_ShouldContainUserMessage()
    {
        var strategy = new DefaultPromptStrategy();

        var result = strategy.BuildPrompt("Привет");

        Assert.Contains("Пользователь: Привет", result);
    }

    [Fact]
    public void BuildPrompt_ShouldContainAssistantLabel()
    {
        var strategy = new DefaultPromptStrategy();

        var result = strategy.BuildPrompt("Тест");

        Assert.Contains("Ассистент:", result);
    }

    [Fact]
    public void BuildPrompt_ShouldContainSystemInstruction()
    {
        var strategy = new DefaultPromptStrategy();

        var result = strategy.BuildPrompt("Hello");

        Assert.Contains("AI-ассистент", result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void BuildPrompt_WithEmptyMessage_ShouldStillBuildPrompt(string input)
    {
        var strategy = new DefaultPromptStrategy();

        var result = strategy.BuildPrompt(input);

        Assert.NotNull(result);
        Assert.Contains("Ассистент:", result);
    }
}