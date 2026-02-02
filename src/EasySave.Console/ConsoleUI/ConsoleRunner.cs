using EasySave.Console.Resources;

namespace EasySave.Console.ConsoleUI;

public class ConsoleRunner
{
    private ITextProvider _texts = new EnglishTextProvider();

    public void RunConsole()
    {
        var menu = new BaseMenu(_texts);
        menu.Display();
    }

    protected void ChangeLanguage(ITextProvider language)
    {
        _texts = language;
        RunConsole();
    }
}