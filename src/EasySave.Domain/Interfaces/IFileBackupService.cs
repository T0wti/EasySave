using EasySave.Domain.Models;

namespace EasySave.Domain.Interfaces
{
    // Interface for managing persistent storage of backup job configurations
    public interface IFileBackupService
    {
        List<BackupJob> LoadJobs();
        void SaveJobs(List<BackupJob> jobs);
    }
}
