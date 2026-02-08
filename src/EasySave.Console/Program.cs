using EasySave.Console;
using EasySave.Application.Controllers;

internal class Program
{
    private static void Main(string[] args)
    {
        var backupController = ControllerFactory.CreateBackupController();
        var configController = ControllerFactory.CreateConfigurationController();

        // MODE COMMANDE
        if (ConsoleCommandRunner.TryRun(args, backupController))
            return;

        // MODE CONSOLE INTERACTIF
        var runner = new ConsoleRunner(backupController, configController);
        runner.RunConsole();
    }
}