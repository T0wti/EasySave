using EasySave.Console.Resources;

namespace EasySave.Console.ConsoleUI;

internal class ExecuteBackupMenu : GeneralContent
{
    internal ExecuteBackupMenu(ITextProvider texts) : base(texts)
    {
    }

    internal void Display()
    {
        DisplayExecuteBackupMenu();
    }

    private void DisplayExecuteBackupMenu()
    {
        Header();
        System.Console.WriteLine(_texts.ExeBackupMenuTitle);
        System.Console.WriteLine();
        System.Console.WriteLine(_texts.BackupName);
        System.Console.WriteLine();
        System.Console.WriteLine(_texts.ExeBackupInstruction);
        Footer();
    }
}