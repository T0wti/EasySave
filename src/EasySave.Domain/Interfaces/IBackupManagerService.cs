using EasySave.Domain.Enums;
using EasySave.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasySave.Domain.Interfaces
{
    public interface IBackupManagerService
    {
        public void CreateBackupJob(string name, string source, string target, BackupType type);

        public void DeleteBackupJob(int id);
        List<BackupJob> GetBackupJobs();

    }
}
