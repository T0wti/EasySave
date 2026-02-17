using EasySave.Application.Resources;

namespace EasySave.Console.ConsoleUI
{

    internal class ExecuteBackupDetail : GeneralContent
    {
        private readonly ITextProvider _texts;
        internal ExecuteBackupDetail(ITextProvider texts) : base(texts)
        {
            _texts = texts;
        }

        internal void Display(int i)
        {
            DisplayExeDetails(i);
        }
        private void DisplayExeDetails(int i)
        {
            Header();
            System.Console.WriteLine(_texts.ExeBackupMenuDetailTitle);
            System.Console.WriteLine();
            // options to display the correct status
            if (i == 0)
            {
                System.Console.WriteLine(_texts.ExeBackupInProgress);
            }
            else
            {
                System.Console.WriteLine(_texts.ExeBackupCompleted);
            }
            Footer();
            if (i == 1)
            {
                Thread.Sleep(2000);
            }
        }
    }
}