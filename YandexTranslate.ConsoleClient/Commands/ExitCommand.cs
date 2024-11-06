using YandexTranslateService;

namespace YandexTranslate.ConsoleClient.Commands;


public class ExitCommand : Command
{
    public override string Name => "exit";

    public override Task ExecuteAsync(Translator.TranslatorClient client)
    {
        Environment.Exit(0);
        return Task.CompletedTask;
    }
}