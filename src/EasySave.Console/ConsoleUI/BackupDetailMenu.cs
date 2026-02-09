using EasySave.Console.Resources;
using EasySave.Application.DTOs;

namespace EasySave.Console.ConsoleUI;

internal class BackupDetailMenu : GeneralContent
{
    private readonly BackupJobDTO _job;

    internal BackupDetailMenu(ITextProvider texts, BackupJobDTO job) : base(texts)
    {
        _job = job;
    }

    internal void Display()
    {
        Header();

        System.Console.WriteLine();

        System.Console.WriteLine($"       {_job.Name}");
        System.Console.WriteLine($"       {_job.SourcePath}");
        System.Console.WriteLine($"       {_job.TargetPath}");
        System.Console.WriteLine($"       {_job.Type}");

        Footer();
    }
}
