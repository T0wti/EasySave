    using EasySave.Domain.Models;

    namespace EasySave.Domain.Interfaces
    {
        public interface IBackupService
        {
            List<BackupJob> GetBackupJobs();
            void CreateBackupJob(BackupJob job);
            void DeleteBackupJob(string jobName);


            void ExecuteBackup(BackupJob job);
            void ExecuteBackups(IEnumerable<BackupJob> jobs);
        }

    }