using EasySave.Application.DTOs;

namespace EasySave.Console.Commands;

internal class EditBackupInteraction
{
    private readonly ConsoleRunner _runner;
    private readonly ConsoleUI.EditBackupMenu _menu;
    private readonly List<BackupJobDTO> _jobs;

    internal EditBackupInteraction(
        ConsoleRunner runner,
        ConsoleUI.EditBackupMenu menu,
        IEnumerable<BackupJobDTO> jobs)
    {
        _runner = runner;
        _menu = menu;
        _jobs = jobs.ToList();
    }

    internal void RunLoop()
    {
        _menu.AskIdToEdit();
        if (!int.TryParse(System.Console.ReadLine(), out int id))
        {
            _runner.WrongInput();
            return;
        }

        var job = _jobs.FirstOrDefault(j => j.Id == id);
        if (job == null)
        {
            _runner.WrongInput();
            return;
        }

        // --- NAME ---
        _menu.ShowCurrentValue(job.Name);
        _menu.AskName();
        var nameInput = System.Console.ReadLine()?.Trim();
        string finalName = string.IsNullOrEmpty(nameInput)
            ? job.Name
            : nameInput;

        // --- SOURCE ---
        _menu.ShowCurrentValue(job.SourcePath);
        _menu.AskSource();
        var sourceInput = System.Console.ReadLine()?.Trim();
        string finalSource = string.IsNullOrEmpty(sourceInput)
            ? job.SourcePath
            : sourceInput;

        // --- TARGET ---
        _menu.ShowCurrentValue(job.TargetPath);
        _menu.AskTarget();
        var targetInput = System.Console.ReadLine()?.Trim();
        string finalTarget = string.IsNullOrEmpty(targetInput)
            ? job.TargetPath
            : targetInput;

        // --- TYPE ---
        _menu.ShowCurrentValue(job.Type.ToString());
        _menu.AskType();
        var typeInput = System.Console.ReadLine()?.Trim();

        int finalTypeChoice;
        if (string.IsNullOrEmpty(typeInput))
        {
            finalTypeChoice = job.Type == "Full" ? 1 : 2;
        }
        else if (!int.TryParse(typeInput, out finalTypeChoice))
        {
            _runner.WrongInput();
            return;
        }

        try
        {
            _runner.HandleEditBackup(
                id,
                finalName,
                finalSource,
                finalTarget,
                finalTypeChoice
            );
        }
        catch (Exception)
        {
            _runner.WrongInput();
        }
    }
}
