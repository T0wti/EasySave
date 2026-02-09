using EasySave.Application.DTOs;
using EasySave.Console.Resources;

namespace EasySave.Console.ConsoleUI;

internal class ExecuteBackupDetail : ExecuteBackupMenu
{
    private readonly List<BackupJobDTO> _jobsList;

    internal ExecuteBackupDetail(ITextProvider texts, IEnumerable<BackupJobDTO> jobs, List<BackupJobDTO> jobs1) : base(texts, jobs)
    {
        _jobsList = jobs1;
    }

    internal void Display()
    {
        DisplayExeDetails();
    }
    private void DisplayExeDetails()
    {
        Header();
        System.Console.WriteLine(_texts.ExeBackupMenuDetailTitle);
        System.Console.WriteLine();
        
        Footer();
    }
}