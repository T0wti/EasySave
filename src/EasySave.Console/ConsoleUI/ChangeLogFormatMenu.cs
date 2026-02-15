using EasySave.Application;
using EasySave.Application.Resources;

namespace EasySave.Console.ConsoleUI;

internal class ChangeLogFormatMenu : GeneralContent
{
    private readonly ConfigAppService _configAppService;
    internal ChangeLogFormatMenu(ITextProvider texts,ConfigAppService configController) : base(texts)
    {
        _configAppService = configController;
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
        System.Console.WriteLine();
        System.Console.Write(_texts.CurrentLogFormat);
        System.Console.Write(_configAppService.GetLogFormat());
        System.Console.WriteLine();
        Footer();
    }
}