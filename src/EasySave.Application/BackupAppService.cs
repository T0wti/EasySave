using EasySave.Application.DTOs;
using EasySave.Application.Exceptions;
using EasySave.Domain.Enums;
using EasySave.Domain.Exceptions;
using EasySave.Domain.Interfaces;
using EasySave.Domain.Models;

namespace EasySave.Application
{
    public class BackupAppService
    {
        private readonly IBackupManagerService _manager;
        private readonly IBackupService _executor;

        public BackupAppService(
            IBackupManagerService manager,
            IBackupService executor)
        {
            _manager = manager;
            _executor = executor;
        }

        // Get all backup jobs as DTOs
        public IEnumerable<BackupJobDTO> GetAll()
        {
            return _manager.GetBackupJobs()
                           .Select(MapToDto);
        }

        // Get a single backup job by Id
        public BackupJobDTO? GetById(int id)
        {
            var job = _manager.GetBackupJobs()
                              .FirstOrDefault(j => j.Id == id);

            return job != null ? MapToDto(job) : null;
        }

        // Create a new backup job
        public void CreateBackup(string name, string source, string target, int typeChoice)
        {
            try
            {
                var type = ConvertTypeChoice(typeChoice);
                _manager.CreateBackupJob(name, source, target, type);
            }
            catch (EasySaveException ex) { throw DomainExceptionMapper.Map(ex); }
        }

        // Edit an existing backup job
        public void EditBackup(int id, string newName, string newSource, string newTarget, int typeChoice)
        {
            try
            {
                var type = ConvertTypeChoice(typeChoice);
                _manager.EditBackupJob(id, newName, newSource, newTarget, type);
            }
            catch (EasySaveException ex) { throw DomainExceptionMapper.Map(ex); }
        }

        // Delete a backup job
        public void DeleteBackup(int id)
        {
            try
            {
                _manager.DeleteBackupJob(id);
            }
            catch (EasySaveException ex) { throw DomainExceptionMapper.Map(ex); }
        }

        // Execute a single backup job asynchronously
        public async Task ExecuteBackupAsync(int id)
        {
            try
            {
                var job = _manager.GetBackupJobs()
                                  .FirstOrDefault(j => j.Id == id);
                await _executor.ExecuteBackupAsync(job);
            }
            catch (EasySaveException ex) { throw DomainExceptionMapper.Map(ex); }
        }

        // Execute multiple backup jobs in parallel
        public async Task ExecuteMultipleAsync(IEnumerable<int> ids)
        {
            try
            {
                var jobs = _manager.GetBackupJobs()
                                   .Where(j => ids.Contains(j.Id));

                await _executor.ExecuteBackupsAsync(jobs);
            }
            catch (AggregateException aex)
            {
                // Unwrap and rethrow the first domain exception found
                var first = aex.InnerExceptions
                    .OfType<EasySaveException>()
                    .FirstOrDefault();

                if (first != null) throw DomainExceptionMapper.Map(first);
                throw;
            }
            catch (EasySaveException ex) { throw DomainExceptionMapper.Map(ex); }
        }

        // Private Methods 

        private static BackupType ConvertTypeChoice(int typeChoice)
        {
            return typeChoice == 1 ? BackupType.Full : BackupType.Differential;
        }

        private static BackupJobDTO MapToDto(BackupJob job)
        {
            return new BackupJobDTO
            {
                Id = job.Id,
                Name = job.Name,
                SourcePath = job.SourcePath,
                TargetPath = job.TargetPath,
                Type = job.Type.ToString()
            };
        }
    }
}