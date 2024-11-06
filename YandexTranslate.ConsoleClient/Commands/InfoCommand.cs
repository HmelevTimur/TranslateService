using Grpc.Core;
using YandexTranslateService;

namespace YandexTranslate.ConsoleClient.Commands;

public class InfoCommand : Command
{
    public override string Name => "info";

    public override async Task ExecuteAsync(Translator.TranslatorClient client)
    {
        var serviceInfoRequest = new Empty();
        try
        {
            var serviceInfoResponse = await client.GetServiceInfoAsync(serviceInfoRequest);
            Console.WriteLine("Информация о сервисе:");
            Console.WriteLine($"Внешний сервис: {serviceInfoResponse.ExternalService}");
            Console.WriteLine($"Тип кэша: {serviceInfoResponse.CacheType}");
            Console.WriteLine($"Объем кэша: {serviceInfoResponse.CacheSize}");
        }
        catch (RpcException ex)
        {
            Console.WriteLine($"Ошибка при получении информации о сервисе: {ex.Status}");
            Console.WriteLine($"Детали: {ex.Message}");
        }
    }
}