using LocalOllamaBot.Application;
using LocalOllamaBot.Core.Abstractions;
using LocalOllamaBot.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var token = "my-token";

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddHttpClient<IOllamaService, OllamaService>(client =>
        {
            client.BaseAddress = new Uri("http://localhost:11434");
            client.Timeout = TimeSpan.FromMinutes(5);
        });
        
        
        
        services.AddSingleton<IPromptStrategy, DefaultPromptStrategy>();
        services.AddSingleton<IChatService, ChatService>();
        services.AddSingleton<ITelegramBot>(provider =>
        {
            var chatService = provider.GetRequiredService<IChatService>();
            var logger = provider.GetRequiredService<ILogger<TelegramBotService>>();
            return new TelegramBotService(token, chatService, logger);
        });

        services.AddLogging(configure => configure.AddConsole());
    })
    .Build();

var bot = host.Services.GetRequiredService<ITelegramBot>();
Console.WriteLine("Бот запущен. Нажмите Ctrl+C для остановки.");
await bot.StartAsync(CancellationToken.None);