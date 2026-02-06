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
        
        Footer();
    }
}