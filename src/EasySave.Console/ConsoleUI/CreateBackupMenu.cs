using EasySave.Application.Resources;

namespace EasySave.Console.ConsoleUI
{

    internal class CreateBackupMenu : GeneralContent
    {
        internal CreateBackupMenu(ITextProvider texts) : base(texts) { }

        internal void Display()
        {
            Header();
            System.Console.WriteLine();
        }

        internal void AskName() => System.Console.WriteLine(_texts.EnterBackupName);
        internal void AskSource() => System.Console.WriteLine(_texts.EnterSourcePath);
        internal void AskTarget() => System.Console.WriteLine(_texts.EnterTargetPath);
        internal void AskType() => System.Console.WriteLine(_texts.EnterBackupType);
    }
}