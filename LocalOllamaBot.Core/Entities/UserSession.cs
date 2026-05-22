// Простая сущность для хранения данных о сессии пользователя
namespace LocalOllamaBot.Core.Entities;

public class UserSession
{
    // Идентификатор чата в Telegram
    public long ChatId { get; set; }
    
    // Список сообщений пользователя и ответов бота (храним до 20 штук)
    public List<string> History { get; set; } = new();
    
    // Время последней активности
    public DateTime LastActive { get; set; }
}