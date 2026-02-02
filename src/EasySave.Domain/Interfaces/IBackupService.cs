using EasySave.Domain.Models;

namespace EasySave.Domain.Interfaces
{
    public interface IBackupService
    {
        void ExecuteBackup(BackupJob job);
        void ExecuteBackups(IEnumerable<BackupJob> jobs);
    }

}