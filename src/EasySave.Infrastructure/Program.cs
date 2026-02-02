// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

// Code to dev the console (exclude only available here, don't push it)
using EasySave.Console.Resources;
using EasySave.Console.ConsoleUI;

namespace EasySave.Infrastructure;

class Program
{
    static void Main()
    {
        ITextProvider texts = new EnglishTextProvider();
        var menu = new BaseMenu(texts);

        menu.Display();
    }
}
