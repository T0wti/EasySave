using EasySave.Domain.Enums;
using EasySave.Domain.Interfaces;
using EasySave.Domain.Models;

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

        public void CreateBackup(string name, string source, string target, int typeChoice)
        {
            var type = (typeChoice == 1) ? BackupType.Full : BackupType.Differential;
            _manager.CreateBackupJob(name, source, target, type);
        }

        public void DeleteBackup(int id)
        {
            _manager.DeleteBackupJob(id);
        }

        public List<BackupJob> GetAll()
        {
            return _manager.GetBackupJobs();
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
    }
}
