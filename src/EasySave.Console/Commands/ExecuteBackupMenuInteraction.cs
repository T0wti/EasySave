using EasySave.Application.DTOs;
using EasySave.Application.Utils;
using EasySave.Console.Controllers;

namespace EasySave.Console.Commands;

internal class ExecuteBackupMenuInteraction
{
    private readonly ConsoleRunner _runner;
    private readonly List<BackupJobDTO> _jobs;
    private readonly BackupController _backupController;

    public ExecuteBackupMenuInteraction(
        ConsoleRunner runner,
        IEnumerable<BackupJobDTO> jobs,
        BackupController backupController)
    {
        _runner = runner;
        _jobs = jobs.ToList();
        _backupController = backupController;
    }

    // Loop to read the input in the interface for the Execute Backup menu
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
                _runner.RunBaseMenu();
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

                _backupController.HandleExecuteMultiple(input);
                exit = true;
                _runner.RunBaseMenu();
            }
            catch
            {
                _runner.WrongInput();
            }
        }
    }
}