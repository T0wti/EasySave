using EasySave.Console;

namespace EasySave.Console.Commands;

public class ListBackupMenuInteraction
{
    private readonly ConsoleRunner _runner;

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

            if (input == "0" || input == "exit")
            {
                exit = true;
                _runner.RunConsole();
            }
            else if (int.TryParse(input, out int id))
            {
                _runner.HandleShowBackupDetail(id);
            }
            else
            {
                _runner.WrongInput();
            }
        }
    }
}
