using EasySave.Console.Resources;

namespace EasySave.Console.ConsoleUI;

internal class FirstStartMenu : GeneralContent
{
    internal FirstStartMenu(ITextProvider texts) : base(texts)
    {
    }
    
    internal void Display()
    {
        DisplayFirstStartMenu();
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
}