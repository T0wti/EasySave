using EasySave.Domain.Models;

namespace EasySave.Domain.Interfaces
{
    // Interface for executing backup jobs
    public interface IBackupService
    {
        Task ExecuteBackupAsync(BackupJob job);
        Task ExecuteBackupsAsync(IEnumerable<BackupJob> jobs);
    }
}