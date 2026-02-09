using EasySave.Domain.Enums;
using EasySave.Domain.Interfaces;
using EasySave.Domain.Models;   
using System;
using System.Collections.Generic;
using System.Text;
using static System.Reflection.Metadata.BlobBuilder;

namespace EasySave.Domain.Services
{
    // Service responsible for managing backup jobs: creation, deletion, editing, and execution
    public class BackupManagerService : IBackupManagerService
    {
        private readonly IFileBackupService _fileBackupService;
        private readonly IBackupService _backupService; 
        private readonly ApplicationSettings _settings; 
        private List<BackupJob> _backupJobs;

        // Constructor: inject dependencies and load existing jobs
        public BackupManagerService(IFileBackupService fileBackupService, IBackupService backupService, ApplicationSettings settings)
        {
            _fileBackupService = fileBackupService;
            _backupService = backupService;
            _settings = settings;

            _backupJobs = _fileBackupService.LoadJobs() ?? new List<BackupJob>();
        }

        // Creates a new backup job with validation
        public void CreateBackupJob(string name, string source, string target, BackupType type)
        {
            // Validate that the source and target paths are absolute
            if (!Path.IsPathRooted(source))
                throw new Exception("Source path format is invalid.");

            if (!Path.IsPathRooted(target))
                throw new Exception("Target path format is invalid.");

            // Ensure job name is unique
            if (_backupJobs.Any(j => j.Name == name))
                throw new Exception($"A job named '{name}' already exists.");

            // Ensure we do not exceed the maximum allowed backup jobs
            if (_backupJobs.Count >= _settings.MaxBackupJobs)
                throw new Exception($"Cannot create more than {_settings.MaxBackupJobs} backup jobs.");

            // Generate the next available ID
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

            // Create new backup job and add to the list
            var job = new BackupJob(nextId, name, source, target, type);
            _backupJobs.Add(job);

            _fileBackupService.SaveJobs(_backupJobs);
        }

        // Deletes a backup job by its ID
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

        // Edits an existing backup job
        public void EditBackupJob(int id, string newName, string newSource, string newTarget, BackupType newType)
        {
            // Find job by ID
            var jobToEdit = _backupJobs.FirstOrDefault(j => j.Id == id);

            // Validate paths
            if (!Path.IsPathRooted(newSource))
                throw new Exception("Source path format is invalid.");

            if (!Path.IsPathRooted(newTarget))
                throw new Exception("Target path format is invalid.");


            if (jobToEdit == null)
                throw new Exception($"A job with ID '{id}' does not exist.");

            // Ensure the new name does not conflict with another job
            if (_backupJobs.Any(j => j.Name == newName && j.Id != id))
                throw new Exception($"A job with {newName} already exists");

            // Update job properties
            jobToEdit.Name = newName;
            jobToEdit.SourcePath = newSource;
            jobToEdit.TargetPath = newTarget;
            jobToEdit.Type = newType;

            // Persist the updated list of jobs
            _fileBackupService.SaveJobs(_backupJobs);
        }

        // Returns the current list of backup jobs
        public List<BackupJob> GetBackupJobs()
        {
            return _backupJobs;
        }

        // Executes a single backup job by ID
        public void ExecuteBackupJob(int id)
        {
            var job = _backupJobs.FirstOrDefault(j => j.Id == id);
            if (job == null)
                throw new Exception("Job not found.");

            _backupService.ExecuteBackup(job);
        }

        // Executes multiple backup jobs sequentially
        public void ExecuteBackupJobs(IEnumerable<BackupJob> jobs)
        {
            foreach(var job in jobs)
            {
                ExecuteBackupJob(job.Id);
            }
        }
    }
}
