namespace LocalOllamaBot.Application;

public interface IPromptStrategy
{
    string BuildPrompt(string userMessage, List<string>? history = null);
}

public class DefaultPromptStrategy : IPromptStrategy
{
    public string BuildPrompt(string userMessage, List<string>? history = null)
    {
        return $"""
                Ты полезный AI-ассистент. Отвечай кратко и дружелюбно.
                Пользователь: {userMessage}
                Ассистент:
                """;
    }
}

public class HistoryAwarePromptStrategy : IPromptStrategy
{
    public string BuildPrompt(string userMessage, List<string>? history)
    {
        var context = (history != null && history.Any())
            ? "История диалога:\n" + string.Join("\n", history.TakeLast(4)) + "\n"
            : "";
        return $"""
                {context}Ты полезный AI-ассистент.
                Пользователь: {userMessage}
                Ассистент:
                """;
    }
}