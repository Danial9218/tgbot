namespace LocalOllamaBot.Core.Entities;

public class UserSession
{
    public long ChatId { get; set; }
    public List<string> History { get; set; } = new();
    public DateTime LastActive { get; set; }
}