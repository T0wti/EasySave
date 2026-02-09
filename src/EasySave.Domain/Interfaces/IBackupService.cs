    using EasySave.Domain.Models;

    namespace EasySave.Domain.Interfaces
    {
    // Interface for executing backup jobs
    public interface IBackupService
        {
            
            void ExecuteBackup(BackupJob job);
            void ExecuteBackups(IEnumerable<BackupJob> jobs);
        }

    }