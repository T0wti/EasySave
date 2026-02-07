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

        string source = "";
        while (true) //To check if source exists
        {
            _menu.AskSource();
            source = System.Console.ReadLine() ?? "";

            if (Directory.Exists(source)) break; //Exit while
            else _runner.WrongInput();
        }

        string target = "";
        while (true)
        {
            _menu.AskTarget();
            target = System.Console.ReadLine() ?? "";

            if (Path.IsPathRooted(target)) break; //If path contains a root (\ or C:)
            else _runner.WrongInput();
        }

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