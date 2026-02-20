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
        private readonly IFileStateService _fileStateService;
        private readonly IBackupHandleRegistry _registry;

        public BackupAppService(
            IBackupManagerService manager,
            IBackupService executor,
            IFileStateService fileStateService,
            IBackupHandleRegistry registry)
        {
            _manager = manager;
            _executor = executor;
            _fileStateService = fileStateService;
            _registry = registry;
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
        public async Task ExecuteBackup(int id)
        {
            try
            {
                var job = _manager.GetBackupJobs()
                                  .FirstOrDefault(j => j.Id == id);

                var handle = new BackupJobHandle();
                _registry.Register(job.Id, handle);

                try
                {
                    await _executor.ExecuteBackup(job, handle);
                }
                finally
                {
                    _registry.Unregister(job.Id);
                }
            }
            catch (OperationCanceledException) { } //A verif
            catch (EasySaveException ex) { throw DomainExceptionMapper.Map(ex); }
        }

        // Execute multiple backup jobs in parallel
        public async Task ExecuteMultiple(IEnumerable<int> ids)
        {
            try
            {
                var jobs = _manager.GetBackupJobs()
                                   .Where(j => ids.Contains(j.Id));

                await _executor.ExecuteBackups(jobs, _registry);
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



        // Pause/Stop : one job
        public void PauseBackup(int jobId) => _registry.Get(jobId)?.Pause();
        public void ResumeBackup(int jobId) => _registry.Get(jobId)?.Resume();
        public void StopBackup(int jobId) => _registry.Get(jobId)?.Stop();

        // Pause/Stop : all jobs
        public void PauseAll()
        {
            foreach (var (_, handle) in _registry.GetAll())
            {
                handle.Pause();
            }
        }
        public void ResumeAll()
        {
            foreach (var (_, handle) in _registry.GetAll())
            {
                handle.Resume();
            }
        }
        public void StopAll()
        {
            foreach (var (_, handle) in _registry.GetAll())
            {
                handle.Stop();
            }
        }

        // Get all jobs progression (dont know if its util)
        public IEnumerable<BackupProgressDTO> GetAllProgress()
        {
            return _fileStateService.ReadState().Select(MapToProgressDto);
        }

        // Get the progression of one job
        public BackupProgressDTO? GetProgress(int jobId)
        {
            var state = _fileStateService.ReadState()
                .FirstOrDefault(s => s.BackupJobId == jobId);

            return state != null ? MapToProgressDto(state) : null;
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

        private static BackupProgressDTO MapToProgressDto(BackupProgress progress) => new()
        {
            BackupJobId = progress.BackupJobId,
            BackupName = progress.BackupName,
            State = progress.State.ToString(),
            Progression = progress.Progression,
            TotalFiles = progress.TotalFiles,
            RemainingFiles = progress.RemainingFiles,
            TotalSize = progress.TotalSize,
            RemainingSize = progress.RemainingSize,
            CurrentSourceFile = progress.CurrentSourceFile,
            CurrentTargetFile = progress.CurrentTargetFile,
            LastUpdate = progress.LastUpdate
        };
    }
}