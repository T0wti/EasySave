using EasySave.Application.Resources;

namespace EasySave.Console.Commands;

internal class FirstStartMenuInteraction
{
    private readonly ITextProvider _texts;
    private readonly ConsoleRunner _runner;

    public FirstStartMenuInteraction(ITextProvider texts, ConsoleRunner runner)
    {
        _texts = texts;
        _runner = runner;
    }

    // Loop to read the input in the interface for the First Start menu
    internal void FirstStartLoop()
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
                    System.Console.Clear();
                    break;
                default:
                    _runner.WrongInput();
                    break;
            }
        }
    }
}