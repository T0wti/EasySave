using EasySave.Console;
using EasySave.Application.Controllers;

internal class Program
{
    private static void Main()
    {
        // Création des controllers via la factory
        var backupController = ControllerFactory.CreateBackupController();
        var configController = ControllerFactory.CreateConfigurationController();

        // Passe les controllers au runner
        var runner = new ConsoleRunner(backupController, configController);

        runner.RunConsole();
    }
}
