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
        var loop = new Commands.BaseMenuInteraction();
        menu.Display();
        loop.RunLoop();
    }

    internal void ChangeLanguage(ITextProvider language)
    {
        _texts = language;
        // ajouter la fonction pour mettre à jour le fichier de config Json
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

}