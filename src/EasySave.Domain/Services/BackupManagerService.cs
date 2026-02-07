using EasySave.Domain.Enums;
using EasySave.Domain.Interfaces;
using EasySave.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasySave.Domain.Services
{
    public class BackupManagerService : IBackupManagerService
    {
        private readonly IFileBackupService _fileBackupService;
        private List<BackupJob> _backupJobs;

        public BackupManagerService(IFileBackupService fileBackupService)
        {
            _fileBackupService = fileBackupService ?? throw new ArgumentNullException(nameof(fileBackupService));
            // Charger les jobs depuis le fichier
            _backupJobs = _fileBackupService.LoadJobs() ?? new List<BackupJob>();
        }

        public void CreateBackupJob(string name, string source, string target, BackupType type)
        {
            if (_backupJobs.Any(j => j.Name == name))
            {
                throw new Exception($"A job named '{name}' already exists.");
            }

            int nextId = _backupJobs.Any() ? _backupJobs.Max(j => j.Id) + 1 : 1;

            var job = new BackupJob(nextId, name, source, target, type);

            _backupJobs.Add(job);
            _fileBackupService.SaveJobs(_backupJobs);
        }

        public void DeleteBackupJob(int jobId) // Changement du type de paramètre (string -> id)
        {
            // Recherche par Id
            var jobToDelete = _backupJobs.FirstOrDefault(j => j.Id == jobId);

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
