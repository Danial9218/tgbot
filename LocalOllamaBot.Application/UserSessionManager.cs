namespace LocalOllamaBot.Application;

// Паттерн Singleton – управляет сессиями пользователей
public sealed class UserSessionManager
{
    // Потокобезопасная ленивая инициализация
    private static readonly Lazy<UserSessionManager> _lazy = new(() => new UserSessionManager());
    public static UserSessionManager Instance => _lazy.Value;

    // Словарь: ключ – ChatId, значение – список сообщений
    private readonly Dictionary<long, List<string>> _history = new();
    private readonly object _lockObject = new(); // Для потокобезопасности

    // Приватный конструктор 
    private UserSessionManager() { }

    // Добавить сообщение в историю 
    public void AddMessage(long chatId, string message)
    {
        lock (_lockObject)
        {
            if (!_history.ContainsKey(chatId))
                _history[chatId] = new List<string>();
            
            _history[chatId].Add(message);
            
            // Ограничиваем длину истории 20 сообщениями
            if (_history[chatId].Count > 20)
                _history[chatId].RemoveAt(0);
        }
    }

    // Получить копию истории для указанного чата
    public List<string> GetHistory(long chatId)
    {
        lock (_lockObject)
        {
            if (_history.TryGetValue(chatId, out var list))
                return list.ToList(); // возвращаем копию, чтобы внешний код не менял оригинал
            return new List<string>();
        }
    }
}