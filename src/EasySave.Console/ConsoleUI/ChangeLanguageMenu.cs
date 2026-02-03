using EasySave.Console.Resources;

namespace EasySave.Console.ConsoleUI;

public class ChangeLanguageMenu : ConsoleRunner
{
    private readonly ITextProvider _texts;

    public ChangeLanguageMenu(ITextProvider texts) 
    {
        _texts = texts;
    }

    public void Display()
    {
        DisplayChangeLanguageBaseMenu();
        LanguageMenuLoop();
    }
    
    private void DisplayChangeLanguageBaseMenu()
    {
        Header();
        System.Console.WriteLine(_texts.LanguageMenuTitle);
        System.Console.WriteLine();
        System.Console.WriteLine(_texts.Language1);
        System.Console.WriteLine(_texts.Language2);
        Footer();
    }
    private void LanguageMenuLoop()
    {
        bool exit = false;
        while (!exit)
        {
            var input = System.Console.ReadLine()?.Trim();

            switch (input)
            {
                case "1":
                    ChangeLanguage(new FrenchTextProvider());
                    exit = true;
                    break;
                case "2":
                    ChangeLanguage(new EnglishTextProvider());
                    exit = true;
                    break;
                case "0":
                    exit = true;
                    RunConsole();
                    break;
                default:
                    System.Console.WriteLine(_texts.WrongInput);
                    break;
            }
        }
    }
}