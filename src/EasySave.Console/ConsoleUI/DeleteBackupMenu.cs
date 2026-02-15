using EasySave.Application.Resources;

namespace EasySave.Console.ConsoleUI;

internal class DeleteBackupMenu : GeneralContent
{
    internal DeleteBackupMenu(ITextProvider texts) : base(texts) { }

    internal void AskIdToDelete() => System.Console.WriteLine(_texts.EnterBackupToDelete);
}