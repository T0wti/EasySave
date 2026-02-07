using EasySave.Console.Resources;

namespace EasySave.Console.ConsoleUI;

internal class ListBackupMenu : GeneralContent
{
    internal ListBackupMenu(ITextProvider texts) : base(texts)
    {
    }

    internal void Display()
    {
        DisplayListBackupMenu();
    }

    private void DisplayListBackupMenu()
    {
        Header();
        System.Console.WriteLine(_texts.ListBackupMenuTitle);
        System.Console.WriteLine();
        System.Console.WriteLine(_texts.BackupNames);
        Footer();
    }
}