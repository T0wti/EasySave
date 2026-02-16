using EasySave.Application.Resources;
using EasySave.Console.Controllers;

namespace EasySave.Console.Commands;

internal class ChangeLanguageMenuInteraction
{
    private readonly ConsoleRunner _runner;
    private readonly ConfigController _configController;

    public ChangeLanguageMenuInteraction(ConsoleRunner runner, ConfigController configController)
    {
        _runner = runner;
        _configController = configController;
    }

    // Loop to read the input in the interface for the Change Language Menu
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
                    _runner.ChangeLanguage(new FrenchTextProvider(), 0);
                    break;
                case "2":
                    exit = true;
                    _runner.ChangeLanguage(new EnglishTextProvider(), 1);
                    break;
                case "0":
                case "exit":
                    exit = true;
                    _runner.RunBaseMenu();
                    break;
                default:
                    _runner.WrongInput();
                    break;
            }
        }
    }
}