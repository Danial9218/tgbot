using LocalOllamaBot.Application;
using Xunit;

namespace LocalOllamaBot.Tests;

public class HistoryAwarePromptStrategyTests
{
    [Fact]
    public void BuildPrompt_WhenNoHistory_ReturnsOnlyUserMessage()
    {
        var strategy = new HistoryAwarePromptStrategy();
        string result = strategy.BuildPrompt("Привет", null);
        Assert.Contains("Пользователь: Привет", result);
        Assert.DoesNotContain("История диалога", result);
    }

    [Fact]
    public void BuildPrompt_WhenEmptyHistory_ReturnsOnlyUserMessage()
    {
        var strategy = new HistoryAwarePromptStrategy();
        string result = strategy.BuildPrompt("Привет", new List<string>());
        Assert.Contains("Пользователь: Привет", result);
        Assert.DoesNotContain("История диалога", result);
    }

    [Fact]
    public void BuildPrompt_WithHistory_TakesLast4Messages()
    {
        var strategy = new HistoryAwarePromptStrategy();
        var history = new List<string> { "msg1", "msg2", "msg3", "msg4", "msg5", "msg6" };
        string result = strategy.BuildPrompt("Новый вопрос", history);
        Assert.Contains("История диалога", result);
        Assert.Contains("msg3", result); // 4-е с конца
        Assert.Contains("msg6", result);
        Assert.DoesNotContain("msg1", result);
        Assert.DoesNotContain("msg2", result);
    }
}