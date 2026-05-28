using LocalOllamaBot.Core.Entities;
using Xunit;



namespace LocalOllamaBot.Tests;

    

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

