using EasySave.Application.DTOs;

namespace EasySave.Console.Commands;

internal class EditBackupInteraction
{
    private readonly ConsoleRunner _runner;
    private readonly ConsoleUI.EditBackupMenu _menu;
    private readonly List<BackupJobDTO> _jobs;

    internal EditBackupInteraction(ConsoleRunner runner, ConsoleUI.EditBackupMenu menu, IEnumerable<BackupJobDTO> jobs)
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

        _menu.ShowCurrentValue(job.Name);
        _menu.AskName();
        var nameInput = System.Console.ReadLine()?.Trim();
        string finalName = string.IsNullOrEmpty(nameInput) ? job.Name : nameInput;

        // Modifier la SOURCE
        _menu.ShowCurrentValue(job.SourcePath);
        _menu.AskSource();
        var sourceInput = System.Console.ReadLine()?.Trim();
        string finalSource = string.IsNullOrEmpty(sourceInput) ? job.SourcePath : sourceInput;

        // Modifier la CIBLE
        _menu.ShowCurrentValue(job.TargetPath);
        _menu.AskTarget();
        var targetInput = System.Console.ReadLine()?.Trim();
        string finalTarget = string.IsNullOrEmpty(targetInput) ? job.TargetPath : targetInput;

        // Modifier le TYPE
        _menu.ShowCurrentValue(job.Type.ToString());
        _menu.AskType();
        var typeInput = System.Console.ReadLine()?.Trim();
        int finalTypeChoice;

        if (string.IsNullOrEmpty(typeInput))
        {
            // On convertit l'enum actuel en int (1 pour Full, 2 pour Differential par exemple)
            finalTypeChoice = job.Type == "Full" ? 1 : 2;
        }
        else
        {
            int.TryParse(typeInput, out finalTypeChoice);
        }

        // Envoi au Runner pour traitement final
        _runner.HandleEditBackup(id, finalName, finalSource, finalTarget, finalTypeChoice);
    }
}