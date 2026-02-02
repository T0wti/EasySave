using EasySave.Console.Resources;
namespace EasySave.Console.ConsoleUI;

public class Menu
{
    private readonly ITextProvider _texts;
    
    public Menu(ITextProvider texts)
    {
        _texts = texts;
    }

    public void DisplayMenu()
    {
        System.Console.Clear();
        System.Console.WriteLine();
        System.Console.WriteLine(_texts.Header);
        System.Console.WriteLine(_texts.MainMenuTitle);
        System.Console.WriteLine();
        System.Console.WriteLine(_texts.BackupOption);
        System.Console.WriteLine(_texts.LanguageOption);
        System.Console.WriteLine(_texts.ExitOption);
        System.Console.WriteLine();
        System.Console.WriteLine(_texts.Footer);
        System.Console.WriteLine();
        System.Console.WriteLine(_texts.AskEntryFromUser);
    }
}