using EasySave.Application.Resources;

namespace EasySave.Console.Commands;

internal class FirstStartMenuInteraction
{
    private readonly ITextProvider _texts;
    private readonly ConsoleRunner _runner;
    private readonly Controllers.ConfigController _configController;

    public FirstStartMenuInteraction(
        ITextProvider texts,
        ConsoleRunner runner,
        Controllers.ConfigController configController)
    {
        _texts = texts;
        _runner = runner;
        _configController = configController;
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
                    exit = true;
                    _runner.ChangeLanguage(new FrenchTextProvider(), 0);
                    break;
                case "2":
                    exit = true;
                    _runner.ChangeLanguage(new EnglishTextProvider(), 1);
                    break;
                case "0":
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