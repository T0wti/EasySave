using EasySave.Application.Resources;
using EasySave.Console.Controllers;

namespace EasySave.Console.ConsoleUI;

internal class ChangeLogFormatMenu : GeneralContent
{
    private readonly ConfigController _configController;

    internal ChangeLogFormatMenu(ITextProvider texts, ConfigController configController) : base(texts)
    {
        _configController = configController;
    }

    internal void Display()
    {
        Header();
        System.Console.WriteLine(_texts.LogFormatMenuTitle);
        System.Console.WriteLine();
        System.Console.WriteLine(_texts.LogFormat1);
        System.Console.WriteLine(_texts.LogFormat2);
        System.Console.WriteLine();
        System.Console.Write(_texts.CurrentLogFormat);
        System.Console.Write(_configController.GetLogFormat());
        System.Console.WriteLine();
        Footer();
    }
}