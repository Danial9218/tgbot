namespace LocalOllamaBot.Core.Abstractions;

public interface IOllamaService
{
    Task<string> GenerateResponseAsync(string prompt, CancellationToken cancellationToken);
    Task<bool> IsModelAvailableAsync(CancellationToken cancellationToken);
}