using EasySave.Application;
using EasySave.Application.Utils;
using EasySave.Application.Resources;
using EasySave.Console;
using EasySave.Console.Controllers;

internal class Program
{
    private static void Main(string[] args)
    {
        var backupAppService = AppServiceFactory.CreateBackupController();
        var configAppService = AppServiceFactory.CreateConfigurationController();

        var settings = configAppService.FileExists()
            ? configAppService.Load()
            : null;

        ITextProvider texts = settings?.LanguageCode == 0
            ? new FrenchTextProvider()
            : new EnglishTextProvider();

        var backupController = new BackupController(backupAppService, texts);
        var configController = new ConfigController(configAppService, texts);

        if (CommandRunner.TryRun(args, backupAppService))
            return;

        var runner = new ConsoleRunner(backupController, configController, texts);
        runner.Run();
    }
}