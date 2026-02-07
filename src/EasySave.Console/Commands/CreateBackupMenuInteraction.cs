namespace EasySave.Console.Commands;

internal class CreateBackupMenuInteraction
{
    private readonly ConsoleRunner _runner;
    private readonly ConsoleUI.CreateBackupMenu _menu;

    internal CreateBackupMenuInteraction(ConsoleRunner runner, ConsoleUI.CreateBackupMenu menu)
    {
        _runner = runner;
        _menu = menu;
    }

    internal void RunLoop()
    {
        _menu.AskName();
        var name = System.Console.ReadLine() ?? "";

        _menu.AskSource();
        var source = System.Console.ReadLine() ?? "";

        _menu.AskTarget();
        var target = System.Console.ReadLine() ?? "";

        _menu.AskType();
        if (!int.TryParse(System.Console.ReadLine(), out int typeChoice))
        {
            _runner.WrongInput();
            _runner.RunBaseMenu();
            return;
        }

        // On passe les types primitifs au runner
        _runner.HandleCreateBackup(name, source, target, typeChoice);
    }
}