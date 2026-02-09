using EasySave.Application.DTOs;
using EasySave.Application.Utils;


namespace EasySave.Console.Commands;

internal class ExecuteBackupMenuInteraction
{
    private readonly ConsoleRunner _runner;
    private readonly List<BackupJobDTO> _jobs;

    public ExecuteBackupMenuInteraction(ConsoleRunner runner, IEnumerable<BackupJobDTO> jobs)
    {
        _runner = runner;
        _jobs = jobs.ToList();
    }

    internal void RunLoop()
    {
        bool exit = false;
        while (!exit)
        {
            var input = System.Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(input))
            {
                _runner.WrongInput();
                continue;
            }

            if (input == "0" || input == "exit")
            {
                exit = true;
                _runner.RunConsole();
                continue;
            }

            try
            {
                var ids = BackupIdParser.ParseIds(input);

                var validIds = _jobs.Select(j => j.Id).ToHashSet();

                if (!ids.All(id => validIds.Contains(id)))
                {
                    _runner.WrongInput();
                    continue;
                }

                _runner.HandleExecuteMultiple(ids);
            }
            catch
            {
                _runner.WrongInput();
            }
        }
    }
}