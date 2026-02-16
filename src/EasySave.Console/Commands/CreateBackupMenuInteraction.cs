using EasySave.Console.Controllers;

namespace EasySave.Console.Commands;

internal class CreateBackupMenuInteraction
{
    private readonly ConsoleRunner _runner;
    private readonly ConsoleUI.CreateBackupMenu _menu;
    private readonly BackupController _backupController;

    internal CreateBackupMenuInteraction(
        ConsoleRunner runner,
        ConsoleUI.CreateBackupMenu menu,
        BackupController backupController)
    {
        _runner = runner;
        _menu = menu;
        _backupController = backupController;
    }

    // Loop to read the input in the interface for the Create Backup menu
    internal void RunLoop()
    {
        _menu.AskName();
        var name = System.Console.ReadLine() ?? "";
        if (name == "0" || name == "exit") { _runner.RunBaseMenu(); return; }

        _menu.AskSource();
        var source = System.Console.ReadLine() ?? "";
        if (source == "0" || source == "exit") { _runner.RunBaseMenu(); return; }

        _menu.AskTarget();
        var target = System.Console.ReadLine() ?? "";
        if (target == "0" || target == "exit") { _runner.RunBaseMenu(); return; }

        _menu.AskType();
        if (!int.TryParse(System.Console.ReadLine(), out int typeChoice))
        {
            _runner.WrongInput();
            _runner.RunCreateBackupMenu();
            return;
        }

        _backupController.HandleCreateBackup(name, source, target, typeChoice);
        _runner.RunBaseMenu();
    }
}