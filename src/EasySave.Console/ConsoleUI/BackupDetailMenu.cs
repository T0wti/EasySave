using EasySave.Console.Resources;

namespace EasySave.Console.ConsoleUI;

internal class BackupDetailMenu : GeneralContent
{
    internal BackupDetailMenu(ITextProvider texts) : base(texts)
    {
    }

    internal void Display()
    {
        DisplayBackupDetail();
    }

    private void DisplayBackupDetail()
    {
        Header();
        System.Console.WriteLine(_texts.BackupNameMenuTitle);
        System.Console.WriteLine();
        System.Console.WriteLine(_texts.BackupName);
        System.Console.WriteLine(_texts.BackupSourcePath);
        System.Console.WriteLine(_texts.BackupTargetPath);
        System.Console.WriteLine(_texts.BackupType);
        Footer();
    }
}