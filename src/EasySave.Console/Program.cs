using EasySave.Application;
using EasySave.Console.Commands;

namespace EasySave.Console;

internal class Program
{
    private static void Main(string[] args)
    {
        var backupController = AppServiceFactory.CreateBackupController();
        var configController = AppServiceFactory.CreateConfigurationController();

        // MODE COMMANDE
        if (ConsoleCommandRunner.TryRun(args, backupController))
            return;

        // MODE CONSOLE INTERACTIF
        var runner = new ConsoleRunner(backupController, configController);
        runner.RunConsole();
    }
}