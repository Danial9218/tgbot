namespace LocalOllamaBot.Application;

public sealed class UserSessionManager
{
    private static readonly Lazy<UserSessionManager> _lazy = new(() => new UserSessionManager());
    public static UserSessionManager Instance => _lazy.Value;

    private readonly Dictionary<long, List<string>> _history = new();
    private readonly object _lock = new();

    private UserSessionManager() { }

    public void AddMessage(long chatId, string message)
    {
        lock (_lock)
        {
            if (!_history.ContainsKey(chatId))
                _history[chatId] = new List<string>();
            _history[chatId].Add(message);
            if (_history[chatId].Count > 20)
                _history[chatId].RemoveAt(0);
        }
    }

    public List<string> GetHistory(long chatId)
    {
        lock (_lock)
        {
            return _history.TryGetValue(chatId, out var list) ? list.ToList() : new List<string>();
        }
    }
}