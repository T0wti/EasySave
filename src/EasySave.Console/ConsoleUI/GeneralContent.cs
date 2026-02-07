using EasySave.Console.Resources;

namespace EasySave.Console.ConsoleUI;

public class GeneralContent : ConsoleRunner
{
    protected readonly ITextProvider _texts;
    internal GeneralContent(ITextProvider texts)
    {
        _texts = texts;
    }
    protected void Header()
    {
        System.Console.Clear();
        System.Console.WriteLine();
        System.Console.WriteLine(_texts.Header);
    }
    protected void Footer()
    {
        System.Console.WriteLine();
        System.Console.WriteLine(_texts.ExitOption);
        System.Console.WriteLine();
        System.Console.WriteLine(_texts.Footer);
        System.Console.WriteLine();
        System.Console.WriteLine(_texts.AskEntryFromUser);
    }
}