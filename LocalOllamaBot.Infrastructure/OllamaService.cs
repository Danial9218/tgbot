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
        var request = new
        {
            model = "qwen3.5:latest",
            prompt = prompt,
            stream = false,
            options = new { temperature = 0.7, max_tokens = 500 }
        };
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync(GenerateUrl, content, cancellationToken);
            response.EnsureSuccessStatusCode();
            var ollamaResponse = await response.Content.ReadFromJsonAsync<OllamaGenerateResponse>(cancellationToken);
            return ollamaResponse?.response ?? "Нет ответа от модели.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обращении к Ollama");
            return "Ошибка: не удалось подключиться к Ollama. Запустите `ollama serve`.";
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

    private class OllamaGenerateResponse
    {
        public string response { get; set; } = string.Empty;
    }
}