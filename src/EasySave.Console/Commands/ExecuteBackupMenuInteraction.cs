using EasySave.Console.Resources;
using EasySave.Console.ConsoleUI;

namespace EasySave.Console.Commands;

internal class ExecuteBackupMenuInteraction
{
    private readonly ConsoleRunner _runner = new();
    private string[]? _inputSplit;

    internal void RunLoop()
    {
        bool exit = false;
        while (!exit)
        {
            var input = System.Console.ReadLine()?.Trim();
            _inputSplit = input?.Split(':');
            switch (_inputSplit[0])
            {
                case "1":
                    System.Console.WriteLine("ça");
                    // fonction d'argan le goat pour dire qu'on prend la save 1
                    break;
                case "2":
                    // fonction d'argan le goat pour dire qu'on prend la save 2
                    break;
                case "3":
                    // fonction d'argan le goat pour dire qu'on prend la save x
                    break;
                case "4":
                    // fonction d'argan le goat pour dire qu'on prend la save x
                    break;
                case "5":
                    // fonction d'argan le goat pour dire qu'on prend la save x
                    break;
                case "0":
                    exit = true;
                    _runner.RunConsole();
                    break;
                default:
                    _runner.WrongInput();
                    break;
            }

            switch (_inputSplit[1])
            {
                case "1":
                    System.Console.WriteLine("marche");
                    // fonction d'argan le goat pour dire qu'on prend la save x
                    break;
                case "2":
                    // fonction d'argan le goat pour dire qu'on prend la save x
                    break;
                case "0":
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