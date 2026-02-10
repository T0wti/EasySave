using EasySave.Console.Resources;

namespace EasySave.Console.ConsoleUI;

internal class ChangeLogFormatMenu : GeneralContent
{
    internal ChangeLogFormatMenu(ITextProvider texts) : base(texts)
    {
    }

    internal void Display()
    {
        DisplayChangeLogFormatBaseMenu();
    }

    private void DisplayChangeLogFormatBaseMenu()
    {
        Header();
        System.Console.WriteLine(_texts.LogFormatMenuTitle);
        System.Console.WriteLine();

        System.Console.WriteLine(_texts.LogFormat1);
        System.Console.WriteLine(_texts.LogFormat2);
        Footer();
    }
}