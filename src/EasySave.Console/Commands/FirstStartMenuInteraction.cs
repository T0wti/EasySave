using EasySave.Console;
using EasySave.Console.Resources;

internal class FirstStartMenuInteraction
{
    private readonly ITextProvider _texts;
    private readonly ConsoleRunner _runner;

    // Passer le runner déjà existant au constructeur
    public FirstStartMenuInteraction(ITextProvider texts, ConsoleRunner runner)
    {
        _texts = texts;
        _runner = runner;
    }

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
    