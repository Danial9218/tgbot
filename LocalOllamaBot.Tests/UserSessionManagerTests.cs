using LocalOllamaBot.Application;
using Xunit;

namespace LocalOllamaBot.Tests;

public class UserSessionManagerTests
{
    [Fact]
    public void AddMessage_WhenHistoryExceeds20_RemovesOldest()
    {
        var manager = UserSessionManager.Instance;
        long chatId = 999;
        for (int i = 0; i < 21; i++)
            manager.AddMessage(chatId, $"msg{i}");
        var history = manager.GetHistory(chatId);
        Assert.Equal(20, history.Count);
        Assert.DoesNotContain("msg0", history);
        Assert.Contains("msg20", history);
    }

    [Fact]
    public void GetHistory_WhenChatIdNotFound_ReturnsEmptyList()
    {
        var manager = UserSessionManager.Instance;
        var history = manager.GetHistory(888);
        Assert.Empty(history);
    }
}