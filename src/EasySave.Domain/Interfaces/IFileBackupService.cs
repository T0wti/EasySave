using System;
using System.Collections.Generic;
using System.Text;
using EasySave.Domain.Models;

namespace EasySave.Domain.Interfaces
{
    public interface IFileBackupService
    {
        List<BackupJob> LoadJobs();
        void SaveJobs(List<BackupJob> jobs);
    }
}
