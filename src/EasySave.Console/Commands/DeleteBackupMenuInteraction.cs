using EasySave.Console.Controllers;

namespace EasySave.Console.Commands;

internal class DeleteBackupMenuInteraction
{
    private readonly ConsoleRunner _runner;
    private readonly ConsoleUI.DeleteBackupMenu _menu;
    private readonly BackupController _backupController;

    internal DeleteBackupMenuInteraction(
        ConsoleRunner runner,
        ConsoleUI.DeleteBackupMenu menu,
        BackupController backupController)
    {
        _runner = runner;
        _menu = menu;
        _backupController = backupController;
    }

    // Loop to read the input in the interface for the Delete Backup menu
    internal void RunLoop()
    {
        _menu.AskIdToDelete();
        var input = System.Console.ReadLine()?.Trim();

        if (input == "0") { _runner.RunBaseMenu(); return; }

        if (int.TryParse(input, out int id))
        {
            _backupController.HandleDeleteBackup(id);
            _runner.RunBaseMenu();
        }
        else
        {
            _runner.WrongInput();
            _runner.RunDeleteBackupMenu();
        }
    }
}