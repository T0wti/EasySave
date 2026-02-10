using EasySave.Console.Resources;

namespace EasySave.Console.Commands;

internal class ChangeLogFormatMenuInteraction
{
    private readonly ConsoleRunner _runner;

    public ChangeLogFormatMenuInteraction(ConsoleRunner runner)
    {
        _runner = runner;
    }

    // Loop to read the input in the interface for the Change Log Format Menu
    internal void RunLoop()
    {
        bool exit = false;
        while (!exit)
        {
            var input = System.Console.ReadLine()?.Trim();
            switch (input)
            {
                case "1":
                    _runner.HandleChangeLogFormat(0); // JSON
                    exit = true;
                    break;
                case "2":
                    _runner.HandleChangeLogFormat(1); // XML
                    exit = true;
                    break;
                case "0":
                    exit = true;
                    _runner.RunConsole();
                    break;
                case "exit":
                    exit = true;
                    _runner.RunConsole();
                    break;
                default:
                    _runner.WrongInput();
                    break;
            }
        }
    }
}