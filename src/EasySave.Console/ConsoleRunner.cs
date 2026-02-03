using EasySave.Console.Resources;

namespace EasySave.Console;

public class ConsoleRunner
{
    private ITextProvider _texts = new EnglishTextProvider();

    public void RunConsole()
    {
        RunBaseMenu();
    }

    internal void RunBaseMenu()
    {
        var menu = new ConsoleUI.BaseMenu(_texts);
        var loop = new Commands.BaseMenuInteraction(_texts);
        menu.Display();
        loop.RunLoop();
    }

    internal void ChangeLanguage(ITextProvider language)
    {
        _texts = language;
        RunBaseMenu();
    }

    internal void RunChangeLanguageMenu()
    {
        var menu = new ConsoleUI.ChangeLanguageMenu(_texts);
        var loop = new Commands.ChangeLanguageMenuInteraction();
        menu.Display();
        loop.RunLoop();
    }

    internal void WrongInput()
    {
        System.Console.WriteLine(_texts.WrongInput);
    }
}