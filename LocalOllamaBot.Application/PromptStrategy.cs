using System.Text;

namespace LocalOllamaBot.Application;

// Паттерн "Стратегия" – определяем интерфейс для построения промпта
public interface IPromptStrategy
{
    string BuildPrompt(string userMessage, List<string>? history = null);
}

// Простая стратегия: без истории, просто инструкция
public class DefaultPromptStrategy : IPromptStrategy
{
    public string BuildPrompt(string userMessage, List<string>? history = null)
    {
        // Формируем промпт для нс
        return $"""
                Ты полезный AI-ассистент. Отвечай кратко и дружелюбно.
                Пользователь: {userMessage}
                Ассистент:
                """;
    }
}

// Расширенная стратегия: учитывает последние 4 сообщения из истории диалога
public class HistoryAwarePromptStrategy : IPromptStrategy
{
    public string BuildPrompt(string userMessage, List<string>? history)
    {
        var sb = new StringBuilder();
        
        // Если история есть и не пустая, добавляем её в промпт
        if (history != null && history.Any())
        {
            sb.AppendLine("История диалога:");
            // Берём только последние 4 сообщения, чтобы не перегружать модель
            var lastMessages = history.TakeLast(4);
            foreach (var msg in lastMessages)
            {
                sb.AppendLine(msg);
            }
            sb.AppendLine();
        }
        
        sb.AppendLine($"Пользователь: {userMessage}");
        sb.Append("Ассистент: ");
        
        return sb.ToString();
    }
}