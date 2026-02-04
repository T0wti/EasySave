using EasySave.Console.Resources;

namespace EasySave.Console;

public class ConsoleRunner
{
    private ITextProvider _texts = new EnglishTextProvider();

    public void RunConsole()
    {
        RunBaseMenu();
    }

    internal void ChangeLanguage(ITextProvider language)
    {
        ChangeLanguagePrivate(language);
    }

    internal void RunChangeLanguageMenu()
    {
        RunChangeLanguageMenuPrivate();
    }

    internal void WrongInput()
    {
        WrongInputPrivate();
    }
    // Methods to call the saves in the infrastrcture
    internal void HandleCreateBackup()
    {
        // Code version to test the interface, not finished
        System.Console.WriteLine("Create backup chosen...");
    }

    internal void HandleDeleteBackup()
    {
        // Code version to test the interface, not finished
        System.Console.WriteLine("Delete backup chosen...");
    }

    internal void HandleEditBackup()
    {
        // Code version to test the interface, not finished
        System.Console.WriteLine("Edit backup chosen...");
    }
    
    // private versions of the interface calling
        private void RunBaseMenu()
        {
            var menu = new ConsoleUI.BaseMenu(_texts);
            var loop = new Commands.BaseMenuInteraction();
            menu.Display();
            loop.RunLoop();
        }

        private void ChangeLanguagePrivate(ITextProvider language)
        {
            _texts = language;
            RunBaseMenu();
        }

        private void RunChangeLanguageMenuPrivate()
        {
            var menu = new ConsoleUI.ChangeLanguageMenu(_texts);
            var loop = new Commands.ChangeLanguageMenuInteraction();
            menu.Display();
            loop.RunLoop();
        }

        private void WrongInputPrivate()
        {
            System.Console.WriteLine(_texts.WrongInput);
        }

}