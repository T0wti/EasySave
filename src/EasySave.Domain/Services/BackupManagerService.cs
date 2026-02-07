using EasySave.Domain.Enums;
using EasySave.Domain.Interfaces;
using EasySave.Domain.Models;

namespace EasySave.Domain.Services
{
    public class BackupManagerService : IBackupManagerService
    {
        private readonly IFileBackupService _fileBackupService;
        private List<BackupJob> _backupJobs;
        private readonly ApplicationSettings _settings;

        public BackupManagerService(IFileBackupService fileBackupService, ApplicationSettings settings)
        {
            _fileBackupService = fileBackupService ?? throw new ArgumentNullException(nameof(fileBackupService));
            _settings = settings;
            // Charger les jobs depuis le fichier
            _backupJobs = _fileBackupService.LoadJobs() ?? new List<BackupJob>();
        }

        public void CreateBackupJob(string name, string source, string target, BackupType type)
        {

            if (_backupJobs.Any(j => j.Name == name))
            {
                throw new Exception($"A job named '{name}' already exists.");
            }

            Console.WriteLine($"DEBUG: _backupJobs.Count = {_backupJobs.Count}, _settings.MaxBackupJobs = {_settings.MaxBackupJobs}");

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
        public List<BackupJob> GetBackupJobs()
        {
            return _backupJobs;
        }
    }
}
