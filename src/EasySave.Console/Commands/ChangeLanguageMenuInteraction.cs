using EasySave.Console.Resources;
using EasySave.Console.ConsoleUI;

namespace EasySave.Console.Commands;

internal class ChangeLanguageMenuInteraction
{
    private readonly ConsoleRunner _runner;

    public ChangeLanguageMenuInteraction(ConsoleRunner runner)
    {
        _runner = runner;
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
                    _runner.ChangeLanguage(new FrenchTextProvider());
                    exit = true;
                    break;
                case "2":
                    _runner.ChangeLanguage(new EnglishTextProvider());
                    exit = true;
                    break;
                case "0":
                    exit = true;
                    _runner.RunConsole();
                    break;
                case "exit":
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
