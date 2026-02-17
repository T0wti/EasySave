using EasySave.Console.Controllers;

namespace EasySave.Console.Commands
{

    internal class ChangeLogFormatMenuInteraction
    {
        private readonly ConsoleRunner _runner;
        private readonly ConfigController _configController;

        public ChangeLogFormatMenuInteraction(ConsoleRunner runner, ConfigController configController)
        {
            _runner = runner;
            _configController = configController;
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
                        exit = true;
                        _configController.HandleChangeLogFormat(0); // JSON
                        _runner.RunBaseMenu();
                        break;
                    case "2":
                        exit = true;
                        _configController.HandleChangeLogFormat(1); // XML
                        _runner.RunBaseMenu();
                        break;
                    case "0":
                    case "exit":
                        exit = true;
                        _runner.RunBaseMenu();
                        break;
                    default:
                        _runner.WrongInput();
                        break;
                }
            }
        }
    }
}