using EasySave.Domain.Interfaces;
using EasySave.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasySave.Domain.Services
{
    internal class BackupManagerService : IBackupManagerService
    {
        private readonly IFileBackupService _fileBackupService;
        private List<BackupJob> _backupJobs;
        public void CreateBackupJob(BackupJob job)
        {
            //Verify if job already exists
            if (_backupJobs.Any(j => j.Name == job.Name))
            {
                throw new Exception($"A job named '{job.Name}' already exists.");
            }

            _backupJobs.Add(job);
            _fileBackupService.SaveJobs(_backupJobs);
        }

        public void DeleteBackupJob(string jobName)
        {
            var jobToDelete = _backupJobs.FirstOrDefault(j => j.Name == jobName);
            if (jobToDelete != null)
            {
                _backupJobs.Remove(jobToDelete);
                _fileBackupService.SaveJobs(_backupJobs);
            }
        }

        public List<BackupJob> GetBackupJobs()
        {
            return _backupJobs;
        }
    }
}
