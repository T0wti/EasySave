using EasySave.Console.ConsoleUI;

namespace EasySave.Console.Commands
{

    internal class ListBackupMenuInteraction
    {
        private readonly ConsoleRunner _runner;
        private readonly ListBackupMenu _menu;

        internal ListBackupMenuInteraction(ConsoleRunner runner, ListBackupMenu menu)
        {
            _runner = runner;
            _menu = menu;
        }

        // Loop to read the input in the interface for the List Backup menu
        internal void RunLoop()
        {
            bool exit = false;
            while (!exit)
            {
                var input = System.Console.ReadLine()?.Trim();

                if (input == "0" || input == "exit")
                {
                    exit = true;
                    _runner.RunBaseMenu();
                }
                else if (int.TryParse(input, out int id))
                {
                    exit = true;
                    _runner.RunBackupDetailMenu(id);
                }
                else
                {
                    _runner.WrongInput();
                }
            }
        }
    }
}