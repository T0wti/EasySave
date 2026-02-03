using EasySave.Console.Resources;

namespace EasySave.Console.ConsoleUI;

public class FirstStartMenu : ConsoleRunner
{
    private readonly ITextProvider _texts;
    public FirstStartMenu(ITextProvider texts)
    {
        _texts = texts;
    }
    
    internal void Display()
    {
        DisplayFirstStartMenu();
        FirstStartLoop();
    }

    private void DisplayFirstStartMenu()
    {
        Header();
        System.Console.WriteLine(_texts.ChooseFirstLanguageMenuTitle);
        System.Console.WriteLine(_texts.ChooseFirstLanguage);
        System.Console.WriteLine(_texts.Language1);
        System.Console.WriteLine(_texts.Language2);
        Footer();
    }

    private void FirstStartLoop()
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
                    System.Console.Clear();
                    break;
                default:
                    System.Console.WriteLine(_texts.WrongInput);
                    break;
            }
        }
    }
}