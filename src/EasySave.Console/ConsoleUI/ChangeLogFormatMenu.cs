using EasySave.Application.Controllers;
using EasySave.Application.Resources;

namespace EasySave.Console.ConsoleUI;

internal class ChangeLogFormatMenu : GeneralContent
{
    private readonly ConfigurationController _configurationController;
    internal ChangeLogFormatMenu(ITextProvider texts,ConfigurationController configController) : base(texts)
    {
        _configurationController = configController;
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
        System.Console.Write(_configurationController.GetLogFormat());
        System.Console.WriteLine();
        Footer();
    }
}