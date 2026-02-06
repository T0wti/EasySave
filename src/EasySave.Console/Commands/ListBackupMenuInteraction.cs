using EasySave.Console.Resources;
using EasySave.Console.ConsoleUI;

namespace EasySave.Console.Commands;

public class ListBackupMenuInteraction
{
    private readonly ConsoleRunner _runner = new();

    internal void RunLoop()
    {
        bool exit = false;
        while (!exit)
        {
            var input=System.Console.ReadLine()?.Trim();

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