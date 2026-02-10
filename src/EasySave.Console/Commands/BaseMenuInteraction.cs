using EasySave.Console.Resources;
using EasySave.Console.ConsoleUI;

namespace EasySave.Console.Commands;

internal class BaseMenuInteraction
{
    private readonly ConsoleRunner _runner;
    private readonly ITextProvider _texts;

    internal BaseMenuInteraction(ConsoleRunner runner, ITextProvider texts)
    {
        _runner = runner;
        _texts = texts;
    }

    // Loop to read the input in the interface for the Base Menu
    internal void RunLoop()
    {
        bool exit = false;
        while (!exit)
        {
            var input = System.Console.ReadLine()?.Trim();

            switch (input)
            {
                case "1":
                    exit = true;
                    _runner.RunCreateBackupMenu();
                    break;
                case "2":
                    exit = true;
                    _runner.RunDeleteBackupMenu();
                    break;
                case "3":;
                    exit = true;
                    _runner.RunEditBackupMenu();
                    break;
                case "4":
                    exit = true;
                    _runner.RunListBackupMenu();
                    break;
                case "5":
                    exit = true;
                    _runner.RunExeBackupMenu();
                    break;
                case "8":
                    exit = true;
                    _runner.RunChangeLogFormatMenu(); // Re do the menu :) !
                    break;
                case "9":
                    exit = true;
                    _runner.RunChangeLanguageMenu();
                    break;
                case "0":
                    exit = true;
                    break;
                default:
                    _runner.WrongInput();
                    break;
            }
        }
    }
}
