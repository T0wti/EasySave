using EasySave.Console.Resources;
using EasySave.Domain.Models;

namespace EasySave.Console.ConsoleUI;

internal class ListBackupMenu : GeneralContent
{
    private readonly List<BackupJob> _jobs;

    internal ListBackupMenu(ITextProvider texts, List<BackupJob> jobs) : base(texts)
    {
        _jobs = jobs;
    }

    internal void Display()
    {
        Header();
        System.Console.WriteLine(_texts.ListBackupMenuTitle);
        System.Console.WriteLine();
        foreach (var job in _jobs)
        {
            System.Console.WriteLine($"{job.Id} | {job.Name} | {job.Type}");
        }
        Footer();
    }
}
