using EasySave.Domain.Models;

namespace EasySave.Domain.Interfaces
{
    // Interface for executing backup jobs
    public interface IBackupService
    {
        Task ExecuteBackup(BackupJob job);
        Task ExecuteBackups(IEnumerable<BackupJob> jobs);
    }
}