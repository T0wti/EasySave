using EasySave.Console;
using EasySave.Console.ConsoleUI;

namespace EasySave.Console.Commands;

public class ListBackupMenuInteraction
{
    private readonly ConsoleRunner _runner;
    private readonly ListBackupMenu _menu;

    internal ListBackupMenuInteraction(ConsoleRunner runner, ListBackupMenu menu)
    {
        _runner = runner;
        _menu = menu;
    }

    internal void RunLoop()
    {
        bool exit = false;
        while (!exit)
        {
            _menu.Display();

            var input = System.Console.ReadLine()?.Trim();

            if (input == "0" || input == "exit")
            {
                exit = true;
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
