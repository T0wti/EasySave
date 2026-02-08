using EasySave.Application.DTOs;
using EasySave.Console.Resources;

namespace EasySave.Console.ConsoleUI;

internal class ListBackupMenu : GeneralContent
{
    private readonly List<BackupJobDTO> _jobs;

    public ListBackupMenu(ITextProvider texts, IEnumerable<BackupJobDTO> jobs)
        : base(texts)
    {
        _jobs = jobs.ToList();
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

        System.Console.WriteLine();

        Footer();
    }
}
