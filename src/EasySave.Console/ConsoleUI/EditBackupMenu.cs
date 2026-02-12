using EasySave.Application.Resources;

namespace EasySave.Console.ConsoleUI;

internal class EditBackupMenu : GeneralContent
{
    internal EditBackupMenu(ITextProvider texts) : base(texts) { }

    internal void AskIdToEdit() => System.Console.WriteLine(_texts.AskIdToEdit);
    internal void AskName() => System.Console.WriteLine(_texts.EnterBackupName);
    internal void AskSource() => System.Console.WriteLine(_texts.EnterSourcePath);
    internal void AskTarget() => System.Console.WriteLine(_texts.EnterTargetPath);
    internal void AskType() => System.Console.WriteLine(_texts.EnterBackupType);

    // Pour afficher la valeur actuelle avant de demander la nouvelle
    internal void ShowCurrentValue(string value) => System.Console.WriteLine($"{value}");
}