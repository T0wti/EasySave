using EasySave.Application.DTOs;
using EasySave.Console.Resources;

namespace EasySave.Console.ConsoleUI;

internal class ExecuteBackupMenu : GeneralContent
{
    private readonly List<BackupJobDTO> _jobs;
    internal ExecuteBackupMenu(ITextProvider texts,IEnumerable<BackupJobDTO> jobs) : base(texts)
    {
        _jobs = jobs.ToList();
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
        
        foreach (var job in _jobs)
        {
            System.Console.WriteLine($"       {job.Id} | {job.Name} | {job.Type}");
        }
        
        // Useless with the actuel display information of a backup
        //System.Console.WriteLine();
        //System.Console.WriteLine(_texts.ExeBackupInstruction);
        Footer();
    }
}