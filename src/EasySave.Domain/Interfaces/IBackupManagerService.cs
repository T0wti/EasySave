using EasySave.Domain.Enums;
using EasySave.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasySave.Domain.Interfaces
{
    public interface IBackupManagerService
    {
        void CreateBackupJob(string name, string source, string target, BackupType type);
        void DeleteBackupJob(int id);
        void EditBackupJob(int id, string newName, string newSource, string newTarget, BackupType newType);
        List<BackupJob> GetBackupJobs();
        void ExecuteBackupJob(int id);
        void ExecuteBackupJobs(IEnumerable<BackupJob> jobs);
    }
}