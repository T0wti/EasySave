using EasySave.Console.Resources;
namespace EasySave.Console.ConsoleUI;

public class BaseMenu
{
    private readonly ITextProvider _texts;
    
    public BaseMenu(ITextProvider texts)
    {
        _texts = texts;
    }

    public void Display()
    {
        DisplayBaseMenu();
    }
    private void DisplayBaseMenu()
    {
        System.Console.Clear();
        System.Console.WriteLine();
        System.Console.WriteLine(_texts.Header);
        System.Console.WriteLine(_texts.MainMenuTitle);
        System.Console.WriteLine();
        System.Console.WriteLine(_texts.CreateBackup);
        System.Console.WriteLine(_texts.DeleteBackup);
        System.Console.WriteLine(_texts.EditBackup);
        System.Console.WriteLine();
        System.Console.WriteLine(_texts.LanguageOption);
        System.Console.WriteLine(_texts.ExitOption);
        System.Console.WriteLine();
        System.Console.WriteLine(_texts.Footer);
        System.Console.WriteLine();
        System.Console.WriteLine(_texts.AskEntryFromUser);
        
        BaseMenuLoop();
    }

    private void BaseMenuLoop()
    {
        bool exit = false;
        while (!exit)
        {
            string input = System.Console.ReadLine();

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
                    var changeLanguageMenu = new ChangeLanguageMenu(_texts);
                    changeLanguageMenu.Display();
                    break;
                case "0":
                    exit = true;
                    break;
                default:
                    System.Console.WriteLine("Invalid input.");
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