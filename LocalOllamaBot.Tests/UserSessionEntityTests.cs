using LocalOllamaBot.Core.Entities;
using Xunit;

public class UserSessionEntityTests
{
    [Fact]
    public void UserSession_PropertiesCanBeSetAndGet()
    {
        var session = new UserSession
        {
            ChatId = 123,
            History = new List<string> { "a", "b" },
            LastActive = DateTime.UtcNow
        };
        Assert.Equal(123, session.ChatId);
        Assert.Equal(2, session.History.Count);
        Assert.NotEqual(default, session.LastActive);
    }
}

/*[Fact]
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
}*/