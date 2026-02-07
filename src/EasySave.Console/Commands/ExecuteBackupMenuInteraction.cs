using EasySave.Application.DTOs;

namespace EasySave.Console.Commands;

internal class ExecuteBackupMenuInteraction
{
    private readonly ConsoleRunner _runner;
    private readonly List<BackupJobDTO> _jobs;

    public ExecuteBackupMenuInteraction(ConsoleRunner runner, IEnumerable<BackupJobDTO> jobs)
    {
        _runner = runner;
        _jobs = jobs.ToList() ;
    }

    internal void RunLoop()
    {
        bool exit = false;
        while (!exit)
        {
            var input = System.Console.ReadLine()?.Trim();

            if (int.TryParse(input, out int choice))
            {
                var job = _jobs.FirstOrDefault(j => j.Id == choice);
                if (job != null)
                {
                    _runner.HandleExecuteBackup(job.Id);
                }
                else
                {
                    _runner.WrongInput();
                }
            }
            else if (input == "0")
            {
                exit = true;
                _runner.RunConsole();
            }
            else
            {
                _runner.WrongInput();
            }
        }
    }
}
