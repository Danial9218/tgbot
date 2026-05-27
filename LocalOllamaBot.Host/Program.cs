using LocalOllamaBot.Application;
using LocalOllamaBot.Core.Abstractions;
using LocalOllamaBot.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

// ТОКЕН ТЕЛЕГРАМ 
string telegramToken = "8793545135:AAEH05RU3D0WLitYVA0osIMGCVN76jZ9wcc"; 

Console.WriteLine("Локальный бот с нейросетью Qwen3.5");
Console.WriteLine("Проверяем подключение к Ollama...");

// Создаём хост приложения
var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Регистрируем HTTP-клиент для работы с Ollama 
        services.AddHttpClient<IOllamaService, OllamaService>(client =>
        {
            client.BaseAddress = new Uri("http://localhost:11434");
            client.Timeout = TimeSpan.FromMinutes(5);
        });

        // Выбираем стратегию построения промпта (с историей или без)
        services.AddSingleton<IPromptStrategy, DefaultPromptStrategy>(); //DefaultPromptStrategy, HistoryAwarePromptStrategy 
        
        // Регистрируем сервис чата
        services.AddSingleton<IChatService, ChatService>();
        
        // Регистрируем Telegram бота
        services.AddSingleton<ITelegramBot>(provider =>
        {
            var chatService = provider.GetRequiredService<IChatService>();
            var logger = provider.GetRequiredService<ILogger<TelegramBotService>>();
            return new TelegramBotService(telegramToken, chatService, logger);
        });

        //НАСТРОЙКА ЛОГИРОВАНИЯ 
        services.AddLogging(configure => 
        {
            configure.AddConsole();
            configure.SetMinimumLevel(LogLevel.Warning); // Показывает только ошибки 
            
            // убрал HTTP и другой мусор
            configure.AddFilter("System.Net.Http.HttpClient", LogLevel.Warning);
            configure.AddFilter("Microsoft", LogLevel.Warning);
        });
        // 
    })
    .Build();

// экземпляр бота и запускаем
var bot = host.Services.GetRequiredService<ITelegramBot>();
Console.WriteLine("Бот запущен. Ожидаю сообщения...");
Console.WriteLine("Нажмите Ctrl+C для остановки.");

await bot.StartAsync(CancellationToken.None);