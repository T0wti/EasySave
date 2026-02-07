using EasySave.Domain.Enums;
using EasySave.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasySave.Domain.Interfaces
{
    public interface IBackupManagerService
    {
        public void CreateBackupJob(BackupJob job);

        public void DeleteBackupJob(string jobName);

        public void EditBackupJob(int id, string newName, string newSource, string newTarget, BackupType newType);
        List<BackupJob> GetBackupJobs();

        public void ExecuteBackupJob(int id);
        public void ExecuteBackupJobs(IEnumerable<BackupJob> jobs);
    }
}
