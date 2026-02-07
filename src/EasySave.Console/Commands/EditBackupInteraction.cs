using EasySave.Domain.Models;

namespace EasySave.Console.Commands;

internal class EditBackupInteraction
{
    private readonly ConsoleRunner _runner;
    private readonly ConsoleUI.EditBackupMenu _menu;
    private readonly List<BackupJob> _jobs;

    internal EditBackupInteraction(ConsoleRunner runner, ConsoleUI.EditBackupMenu menu, List<BackupJob> jobs)
    {
        _runner = runner;
        _menu = menu;
        _jobs = jobs;
    }

    internal void RunLoop()
    {
        // 1. Demander quel Job modifier
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

        // 2. Modifier le NOM
        _menu.ShowCurrentValue(job.Name);
        _menu.AskName();
        var nameInput = System.Console.ReadLine()?.Trim();
        string finalName = string.IsNullOrEmpty(nameInput) ? job.Name : nameInput;

        // 3. Modifier la SOURCE
        _menu.ShowCurrentValue(job.SourcePath);
        _menu.AskSource();
        var sourceInput = System.Console.ReadLine()?.Trim();
        string finalSource = string.IsNullOrEmpty(sourceInput) ? job.SourcePath : sourceInput;

        // 4. Modifier la CIBLE
        _menu.ShowCurrentValue(job.TargetPath);
        _menu.AskTarget();
        var targetInput = System.Console.ReadLine()?.Trim();
        string finalTarget = string.IsNullOrEmpty(targetInput) ? job.TargetPath : targetInput;

        // 5. Modifier le TYPE
        _menu.ShowCurrentValue(job.Type.ToString());
        _menu.AskType();
        var typeInput = System.Console.ReadLine()?.Trim();
        int finalTypeChoice;

        if (string.IsNullOrEmpty(typeInput))
        {
            // On convertit l'enum actuel en int (1 pour Full, 2 pour Differential par exemple)
            finalTypeChoice = (job.Type == Domain.Enums.BackupType.Full) ? 1 : 2;
        }
        else
        {
            int.TryParse(typeInput, out finalTypeChoice);
        }

        // 6. Envoi au Runner pour traitement final
        _runner.HandleEditBackup(id, finalName, finalSource, finalTarget, finalTypeChoice);
    }
}