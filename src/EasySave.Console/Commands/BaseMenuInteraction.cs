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

    internal void RunLoop()
    {
        var menu = new ConsoleUI.BaseMenu(_texts);
        bool exit = false;
        while (!exit)
        {
            menu.Display();
            var input = System.Console.ReadLine()?.Trim();

            switch (input)
            {
                case "1":
                    _runner.RunCreateBackupMenu();
                    break;
                case "2":
                    _runner.RunDeleteBackupMenu();
                    break;
                case "3":
                    _runner.RunEditBackupMenu();
                    break;
                case "4":
                    _runner.RunListBackupMenu();
                    break;
                case "5":
                    _runner.RunExeBackupMenu();
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
