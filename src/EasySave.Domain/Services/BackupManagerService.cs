using EasySave.Domain.Enums;
using EasySave.Domain.Interfaces;
using EasySave.Domain.Models;   
using System;
using System.Collections.Generic;
using System.Text;
using static System.Reflection.Metadata.BlobBuilder;

namespace EasySave.Domain.Services
{
    public class BackupManagerService : IBackupManagerService
    {
        private readonly IFileBackupService _fileBackupService;
        private readonly IBackupService _backupService; 
        private readonly ApplicationSettings _settings; 
        private List<BackupJob> _backupJobs;

        public BackupManagerService(IFileBackupService fileBackupService, IBackupService backupService, ApplicationSettings settings)
        {
            _fileBackupService = fileBackupService ?? throw new ArgumentNullException(nameof(fileBackupService));
            _backupService = backupService ?? throw new ArgumentNullException(nameof(backupService));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));

            _backupJobs = _fileBackupService.LoadJobs() ?? new List<BackupJob>();
        }

        public void CreateBackupJob(string name, string source, string target, BackupType type)
        {

            if (!Path.IsPathRooted(source))
                throw new Exception("Source path format is invalid.");

            if (!Path.IsPathRooted(target))
                throw new Exception("Target path format is invalid.");


            if (_backupJobs.Any(j => j.Name == name))
            {
                throw new Exception($"A job named '{name}' already exists.");
            }

            if (_backupJobs.Count >= _settings.MaxBackupJobs)
            {
                throw new Exception($"Cannot create more than {_settings.MaxBackupJobs} backup jobs.");
            }
           
            int nextId = 1;
            var existingIds = _backupJobs.Select(j => j.Id).OrderBy(id => id).ToList();
            foreach (var id in existingIds)
            {
                if (id == nextId)
                {
                    nextId++;
                }
                else
                {
                    break; 
                }
            }
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
        public void EditBackupJob(int id, string newName, string newSource, string newTarget, BackupType newType)
        {
            //Search job
            var jobToEdit = _backupJobs.FirstOrDefault(j => j.Id == id);

            if (!Path.IsPathRooted(newSource))
                throw new Exception("Source path format is invalid.");

            if (!Path.IsPathRooted(newTarget))
                throw new Exception("Target path format is invalid.");


            if (jobToEdit == null)
            {
                throw new Exception($"A job with ID '{id}' does not exist.");
            }

            //Check if new name does not collide with already existing name
            if (_backupJobs.Any(j => j.Name == newName && j.Id != id))
            {
                throw new Exception($"A job with {newName} already exists");
            }

            jobToEdit.Name = newName;
            jobToEdit.SourcePath = newSource;
            jobToEdit.TargetPath = newTarget;
            jobToEdit.Type = newType;

            _fileBackupService.SaveJobs(_backupJobs);
        }

        public List<BackupJob> GetBackupJobs()
        {
            return _backupJobs;
        }

        public void ExecuteBackupJob(int id)
        {
            var job = _backupJobs.FirstOrDefault(j => j.Id == id);
            if (job == null)
                throw new Exception("Job not found.");

            _backupService.ExecuteBackup(job);
        }

        public void ExecuteBackupJobs(IEnumerable<BackupJob> jobs)
        {
            foreach(var job in jobs)
            {
                ExecuteBackupJob(job.Id);
            }
        }
    }
}
