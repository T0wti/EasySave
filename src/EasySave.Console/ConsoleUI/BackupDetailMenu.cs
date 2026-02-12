using EasySave.Application.DTOs;
using EasySave.Application.Resources;

namespace EasySave.Console.ConsoleUI;

internal class BackupDetailMenu : GeneralContent
{
    private readonly BackupJobDTO _job;

    internal BackupDetailMenu(ITextProvider texts, BackupJobDTO job) : base(texts)
    {
        _job = job; // required to show the job details
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
