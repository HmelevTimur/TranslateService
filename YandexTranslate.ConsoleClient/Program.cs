using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using YandexTranslate.ConsoleClient.Commands;
using YandexTranslateService;

namespace YandexTranslate.ConsoleClient;

class Program
{
    static async Task Main(string[] args)
    {
        var services = new ServiceCollection();
        services.AddGrpcClient<Translator.TranslatorClient>(o => { o.Address = new Uri("http://localhost:5106"); });
        var provider = services.BuildServiceProvider();

        var translateClient = provider.GetRequiredService<Translator.TranslatorClient>();

        var commands = new Dictionary<string, Command>
        {
            { "info", new InfoCommand() },
            { "exit", new ExitCommand() }
        };

        while (true)
        {
            Console.WriteLine(
                "Введите текст для перевода (или 'exit' для выхода, 'info' для получения информации о сервисе):");
            string inputText = Console.ReadLine();

            if (commands.TryGetValue(inputText.ToLower(), out var command))
            {
                await command.ExecuteAsync(translateClient);
                if (command is ExitCommand) break;
                continue;
            }

            Console.WriteLine("Введите исходный язык (например, 'en'):");
            string sourceLang = Console.ReadLine();

            Console.WriteLine("Введите целевой язык (например, 'es'):");
            string targetLang = Console.ReadLine();

            var request = new TranslateRequest
            {
                Texts = { inputText },
                SourceLang = sourceLang,
                TargetLang = targetLang
            };

            try
            {
                var response = await translateClient.TranslateAsync(request);

                Console.WriteLine("Перевод:");
                foreach (var translation in response.Translations)
                {
                    Console.WriteLine(translation);
                }
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"Ошибка: {ex.Status}");
                Console.WriteLine($"Детали: {ex.Message}");
                Console.WriteLine($"Стек вызовов: {ex.StackTrace}");
            }
        }
    }
}