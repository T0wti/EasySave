using EasySave.Console.Resources;

namespace EasySave.Console.ConsoleUI;

internal class ChangeLanguageMenu : GeneralContent
{

    internal ChangeLanguageMenu(ITextProvider texts) : base(texts)
    {
    }

    internal void Display()
    {
        DisplayChangeLanguageBaseMenu();
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
}