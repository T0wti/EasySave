using EasySave.Application.DTOs;
using EasySave.Console.Controllers;

namespace EasySave.Console.Commands;

internal class EditBackupInteraction
{
    private readonly ConsoleRunner _runner;
    private readonly ConsoleUI.EditBackupMenu _menu;
    private readonly List<BackupJobDTO> _jobs;
    private readonly BackupController _backupController;

    internal EditBackupInteraction(
        ConsoleRunner runner,
        ConsoleUI.EditBackupMenu menu,
        IEnumerable<BackupJobDTO> jobs,
        BackupController backupController)
    {
        _runner = runner;
        _menu = menu;
        _jobs = jobs.ToList();
        _backupController = backupController;
    }

    // Loop to read the input in the interface for the Edit Backup menu
    internal void RunLoop()
    {
        var input = System.Console.ReadLine();

        if (input == "0" || input?.Equals("exit", StringComparison.OrdinalIgnoreCase) == true)
        {
            _runner.RunBaseMenu();
            return;
        }

        if (!int.TryParse(input, out int id))
        {
            _runner.WrongInput();
            _runner.RunEditBackupMenu();
            return;
        }

        var job = _jobs.FirstOrDefault(j => j.Id == id);
        if (job == null)
        {
            _runner.WrongInput();
            _runner.RunEditBackupMenu();
            return;
        }

        // --- NAME ---
        _menu.ShowCurrentValue(job.Name);
        _menu.AskName();
        var nameInput = System.Console.ReadLine()?.Trim();
        string finalName = string.IsNullOrEmpty(nameInput) ? job.Name : nameInput;

        // --- SOURCE ---
        _menu.ShowCurrentValue(job.SourcePath);
        _menu.AskSource();
        var sourceInput = System.Console.ReadLine()?.Trim();
        string finalSource = string.IsNullOrEmpty(sourceInput) ? job.SourcePath : sourceInput;

        // --- TARGET ---
        _menu.ShowCurrentValue(job.TargetPath);
        _menu.AskTarget();
        var targetInput = System.Console.ReadLine()?.Trim();
        string finalTarget = string.IsNullOrEmpty(targetInput) ? job.TargetPath : targetInput;

        // --- TYPE ---
        _menu.ShowCurrentValue(job.Type.ToString());
        _menu.AskType();
        var typeInput = System.Console.ReadLine()?.Trim();
        int finalTypeChoice;
        if (string.IsNullOrEmpty(typeInput))
            finalTypeChoice = job.Type == "Full" ? 1 : 2;
        else if (!int.TryParse(typeInput, out finalTypeChoice))
        {
            _runner.WrongInput();
            _runner.RunEditBackupMenu();
            return;
        }

        _backupController.HandleEditBackup(id, finalName, finalSource, finalTarget, finalTypeChoice);
        _runner.RunBaseMenu();
    }
}