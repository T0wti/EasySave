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
        List<BackupJob> GetBackupJobs();

        public void ExecuteBackupJob(int id);
        public void ExecuteBackupJobs();
    }
}
