using EasySave.Application.DTOs;
using EasySave.Domain.Enums;
using EasySave.Domain.Interfaces;

namespace EasySave.Application.Controllers
{
    public class BackupController
    {
        private readonly IBackupManagerService _manager;
        private readonly IBackupService _executor;

        public BackupController(
            IBackupManagerService manager,
            IBackupService executor)
        {
            _manager = manager;
            _executor = executor;
        }

        public IEnumerable<BackupJobDTO> GetAll()
        {
            return _manager.GetBackupJobs()
                .Select(MapToDto);
        }

        public BackupJobDTO? GetById(int id)
        {
            var job = _manager.GetBackupJobs()
                              .FirstOrDefault(j => j.Id == id);

            return job == null ? null : MapToDto(job);
        }

        public void CreateBackup(string name, string source, string target, int typeChoice)
        {
            var type = (typeChoice == 1) ? BackupType.Full : BackupType.Differential;
            _manager.CreateBackupJob(name, source, target, type);
        }

        public void EditBackup(int id, string newName, string newSource, string newTarget, int typeChoice)
        {
            var type = (typeChoice == 1) ? BackupType.Full : BackupType.Differential;
            _manager.EditBackupJob(id, newName, newSource, newTarget, type);
        }

        public void DeleteBackup(int id)
        {
            _manager.DeleteBackupJob(id);
        }

        public void ExecuteBackup(int id)
        {
            var job = _manager.GetBackupJobs()
                              .FirstOrDefault(j => j.Id == id);

            if (job == null)
                throw new Exception("Backup not found");

            _executor.ExecuteBackup(job);
        }

        public void ExecuteMultiple(IEnumerable<int> ids)
        {
            var jobs = _manager.GetBackupJobs()
                               .Where(j => ids.Contains(j.Id));

            _executor.ExecuteBackups(jobs);
        }



        private static BackupJobDTO MapToDto(dynamic job)
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
