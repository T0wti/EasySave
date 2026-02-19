using EasySave.Domain.Enums;
using EasySave.Domain.Models;

namespace EasySave.Domain.Interfaces
{
    // Interface for managing backup jobs: creation, deletion, editing, retrieval, and execution
    public interface IBackupManagerService
    {
        void CreateBackupJob(string name, string source, string target, BackupType type);
        void DeleteBackupJob(int id);
        void EditBackupJob(int id, string newName, string newSource, string newTarget, BackupType newType);
        public IReadOnlyList<BackupJob> GetBackupJobs();
    }
}