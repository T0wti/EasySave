using EasySave.Domain.Enums;
using EasySave.Domain.Exceptions;
using EasySave.Domain.Interfaces;
using EasySave.Domain.Models;
using System.Xml.Linq;


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

            if (string.IsNullOrWhiteSpace(name))
                throw new BackupValidationException("Name", EasySaveErrorCode.NameEmpty);

            // Ensure job name is unique
            if (_backupJobs.Any(j => j.Name == name))
                throw new BackupJobAlreadyExistsException(name);

            // Validate that the source and target paths are absolute
            if (string.IsNullOrWhiteSpace(source))
                throw new BackupValidationException("SourcePath", EasySaveErrorCode.SourcePathEmpty);

            if (!Path.IsPathRooted(source))
                throw new BackupValidationException("SourcePath", EasySaveErrorCode.SourcePathNotAbsolute);

            if (string.IsNullOrWhiteSpace(target))
                throw new BackupValidationException("TargetPath", EasySaveErrorCode.TargetPathEmpty);
            if (!Path.IsPathRooted(target))
                throw new BackupValidationException("TargetPath", EasySaveErrorCode.TargetPathNotAbsolute);

            if (source.Trim().Equals(target.Trim(), StringComparison.OrdinalIgnoreCase))
                throw new BackupValidationException("SourcePath", EasySaveErrorCode.SourceEqualsTarget);

            // Generate the next available ID
            int nextId = 1;
            var existingIds = _backupJobs.Select(j => j.Id).OrderBy(id => id).ToList();
            foreach (var id in existingIds)
            {
                if (id == nextId) nextId++;
                else break;
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
            if (!Path.IsPathRooted(newSource))
                throw new BackupValidationException("SourcePath", EasySaveErrorCode.SourcePathNotAbsolute);

            if (!Path.IsPathRooted(newTarget))
                throw new BackupValidationException("TargetPath", EasySaveErrorCode.TargetPathNotAbsolute);

            var index = _backupJobs.FindIndex(j => j.Id == id);
            if (index == -1)
                throw new BackupJobNotFoundException(id);

            if (_backupJobs.Any(j => j.Name == newName && j.Id != id))
                throw new BackupJobAlreadyExistsException(newName);

            _backupJobs[index] = new BackupJob(id, newName, newSource, newTarget, newType);
            _fileBackupService.SaveJobs(_backupJobs);
        }

        // Returns the current list of backup jobs
        public IReadOnlyList<BackupJob> GetBackupJobs()
        {
            return _backupJobs.AsReadOnly();
        }
        
    }
}
