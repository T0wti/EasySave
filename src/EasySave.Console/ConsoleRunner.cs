using EasySave.Console.Resources;

namespace EasySave.Console;

public class ConsoleRunner
{
    private ITextProvider _texts = new EnglishTextProvider();

    public void RunConsole()
    {
        var menu = new ConsoleUI.BaseMenu(_texts);
        menu.Display();
    }

    protected void ChangeLanguage(ITextProvider language)
    {
        _texts = language;
        RunConsole();
    }
}