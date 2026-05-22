using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using LocalOllamaBot.Core.Abstractions;
using Microsoft.Extensions.Logging;

namespace LocalOllamaBot.Infrastructure;

public class OllamaService : IOllamaService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OllamaService> _logger;
    private const string GenerateUrl = "http://localhost:11434/api/generate";
    private const string TagsUrl = "http://localhost:11434/api/tags";

    public OllamaService(HttpClient httpClient, ILogger<OllamaService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<string> GenerateResponseAsync(string prompt, CancellationToken cancellationToken)
    {
        // Создаём объект запроса для Ollama
        var requestPayload = new
        {
            model = "qwen3.5:latest", // имя модели, которую скачал через ollama 
            prompt = prompt,
            stream = false,          // не стримим, ждём полный ответ
            options = new
            {
                temperature = 0.7,
                max_tokens = 500
            }
        };
        
        var content = new StringContent(JsonSerializer.Serialize(requestPayload), Encoding.UTF8, "application/json");
        
        try
        {
            _logger.LogInformation("Отправка POST запроса в Ollama...");
            var response = await _httpClient.PostAsync(GenerateUrl, content, cancellationToken);
            response.EnsureSuccessStatusCode();
            
            var ollamaResponse = await response.Content.ReadFromJsonAsync<OllamaGenerateResponse>(cancellationToken);
            return ollamaResponse?.response ?? "Ollama вернул пустой ответ!";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при вызове Ollama API");
            return "Ошибка при обращении к Ollama. Убедитесь, что он запущен и модель загружена.";
        }
    }

    public async Task<bool> IsModelAvailableAsync(CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.GetAsync(TagsUrl, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    // Вспомогательный класс для десериализации ответа Ollama
    private class OllamaGenerateResponse
    {
        public string response { get; set; } = string.Empty;
    }
}   