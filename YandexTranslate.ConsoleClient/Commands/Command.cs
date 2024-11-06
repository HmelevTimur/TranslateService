using YandexTranslateService;

namespace YandexTranslate.ConsoleClient.Commands;

public abstract class Command
{
    public abstract string Name { get; }
    public abstract Task ExecuteAsync(Translator.TranslatorClient client);
}