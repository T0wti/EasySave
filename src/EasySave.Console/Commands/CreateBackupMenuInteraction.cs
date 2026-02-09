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

    // Loop to read the input in the interface for the Create Backup menu
    internal void RunLoop() 
    { 
        _menu.AskName();
        var name = System.Console.ReadLine() ?? "";
        if (name == "0" || name == "exit")
        {
            Case0();
            return;
        }

        _menu.AskSource();
        var source = System.Console.ReadLine() ?? "";
        if (source == "0" || source == "exit")
        {
            Case0();
            return;
        }

        _menu.AskTarget();
        var target = System.Console.ReadLine() ?? "";
        if (target == "0" || source == "exit")
        {
            Case0();
            return;
        }

        _menu.AskType();
        if (!int.TryParse(System.Console.ReadLine(), out int typeChoice))
        {
            _runner.WrongInput();
            return;
        }

        try
        {
            _runner.HandleCreateBackup(name, source, target, typeChoice);
        }
        catch (Exception)   
        {
            _runner.WrongInput(); 
        } 
    }

    private void Case0()
    {
        _runner.RunConsole();
    }
}