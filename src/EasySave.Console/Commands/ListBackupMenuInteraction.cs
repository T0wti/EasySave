using EasySave.Console;

namespace EasySave.Console.Commands;

public class ListBackupMenuInteraction
{
    private readonly ConsoleRunner _runner;

    // On reçoit le runner existant
    public ListBackupMenuInteraction(ConsoleRunner runner)
    {
        _runner = runner;
    }

    internal void RunLoop()
    {
        bool exit = false;
        while (!exit)
        {
            var input = System.Console.ReadLine()?.Trim();

            switch (input)
            {
                case "0" or "exit":
                    exit = true;
                    _runner.RunConsole();
                    break;
                default:
                    _runner.WrongInput();
                    break;
            }
        }
    }
}
