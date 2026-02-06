using EasySave.Console.Resources;

namespace EasySave.Console.ConsoleUI;

internal class BaseMenu : GeneralContent
{
    internal BaseMenu(ITextProvider texts) : base(texts)
    {
    }

    internal void Display()
    {
        DisplayBaseMenu();
    }
    private void DisplayBaseMenu()
    {
        Header();
        System.Console.WriteLine(_texts.MainMenuTitle);
        System.Console.WriteLine();
        System.Console.WriteLine(_texts.CreateBackup);
        System.Console.WriteLine(_texts.DeleteBackup);
        System.Console.WriteLine(_texts.EditBackup);
        System.Console.WriteLine(_texts.ListBackup);
        System.Console.WriteLine(_texts.ExeBackup);
        System.Console.WriteLine();
        System.Console.WriteLine(_texts.LanguageOption);
        Footer();
    }
}