using EasySave.Console.Resources;
using EasySave.Console.ConsoleUI;

namespace EasySave.Console.Commands;

internal class BaseMenuInteraction
{
    private readonly ConsoleRunner _runner;
    internal BaseMenuInteraction()
    {
        _runner = new ConsoleRunner();
    }

    internal void RunLoop()
    {
        bool exit = false;
        while (!exit)
        {
            var input = System.Console.ReadLine()?.Trim();

            switch (input)
            {
                case "1":
                    _runner.HandleCreateBackup();
                    break;
                case "2":
                    _runner.HandleDeleteBackup();
                    break;
                case "3":
                    _runner.HandleEditBackup();
                    break;
                case "9":
                    exit = true;
                    _runner.RunChangeLanguageMenu();
                    break;
                case "0":
                    exit = true;
                    break;
                // Case for dev, remove before flight
                case "dev-first-start":
                    var newMenu = new FirstStartMenu(new EnglishTextProvider());
                    newMenu.Display();
                    break;
                // Default case
                default:
                    _runner.WrongInput();
                    break;
            }
        }
    }
}