using EasySave.Console.Resources;
using EasySave.Console.ConsoleUI;

namespace EasySave.Console.Commands;

internal class BaseMenuInteraction
{
    private readonly ITextProvider _texts;
    private readonly ConsoleRunner _runner;
    internal BaseMenuInteraction(ITextProvider texts)
    {
        _texts = texts;
        _runner = new ConsoleRunner();
    }

    internal void RunLoop()
    {
        bool exit = false;
        while (!exit)
        {
            var input = System.Console.ReadLine()?.Trim();

            switch (input)
            {
                case "1":
                    HandleCreateBackup();
                    break;
                case "2":
                    HandleDeleteBackup();
                    break;
                case "3":
                    HandleEditBackup();
                    break;
                case "9":
                    exit = true;
                    _runner.RunChangeLanguageMenu();
                    break;
                case "0":
                    exit = true;
                    break;
                // Case for dev, remove before flight
                case "dev-first-start":
                    var newMenu = new FirstStartMenu(new EnglishTextProvider());
                    newMenu.Display();
                    break;
                // Default case
                default:
                    _runner.WrongInput();
                    break;
            }
        }
    }
    // Méthodes séparées pour chaque action à mettre à jour quand Argan aura cook
    private void HandleCreateBackup()
    {
        System.Console.WriteLine("Create backup chosen...");
        System.Console.ReadKey();
    }

    private void HandleDeleteBackup()
    {
        System.Console.WriteLine("Delete backup chosen...");
        System.Console.ReadKey();
    }

    private void HandleEditBackup()
    {
        System.Console.WriteLine("Edit backup chosen...");
        System.Console.ReadKey();
    }

}