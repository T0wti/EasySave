using EasySave.Domain.Enums;
using EasySave.Domain.Interfaces;
using EasySave.Domain.Models;
using EasySave.EasyLog;
using EasySave.EasyLog.Interfaces;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;

namespace EasySave.Domain.Services
{
    public class BackupService : IBackupService
    {
        private readonly IFileService _fileService;
        private readonly ILogService _logService;
        private readonly IStateService _stateService;
        private readonly IBackupStrategy _fullStrategy;
        private readonly IBackupStrategy _differentialStrategy;

        private readonly IFileBackupService _fileBackupService;
        private List<BackupJob> _backupJobs;


        public BackupService(
        IFileService fileService,
        IBackupStrategy fullStrategy,
        IBackupStrategy differentialStrategy,
        IFileBackupService fileBackupService)
        {
            _fileService = fileService;
            _logService = EasyLogService.Instance;
            _stateService = new StateService(FileStateService.Instance);
            _fullStrategy = fullStrategy;
            _differentialStrategy = differentialStrategy;

            //JSON initialisation
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string easySavePath = Path.Combine(appDataPath, "EasySave");

            if (!Directory.Exists(easySavePath))
            {
                Directory.CreateDirectory(easySavePath);
            }

            _jobsFilePath = Path.Combine(easySavePath, "jobs.json");
            _fileBackupService = fileBackupService;
            _backupJobs = _fileBackupService.LoadJobs();
        }

        public List<BackupJob> GetBackupJobs()
        {
            return _backupJobs;
        }
        
        public void CreateBackupJob(BackupJob job)
        {
            //Verify if job already exists
            if (_backupJobs.Any(j => j.Name == job.Name)) { 
                throw new Exception($"A job named '{job.Name}' already exists.");
            }

            _backupJobs.Add(job);
            _fileBackupService.SaveJobs(_backupJobs);
        }

        public void DeleteBackupJob(string jobName)
        {
            var jobToDelete = _backupJobs.FirstOrDefault(j => j.Name == jobName);
            if (jobToDelete != null) { 
                _backupJobs.Remove(jobToDelete);
                _fileBackupService.SaveJobs(_backupJobs);
            }
        }

        public void ExecuteBackup(BackupJob job)
        {

            IBackupStrategy strategy = job.Type == BackupType.Full
            ? _fullStrategy
            : _differentialStrategy;

            var files = strategy.GetFilesToCopy(job.SourcePath, job.TargetPath);

            var progress = BackupProgress.From(job);

            _stateService.Initialize(progress, files);

            foreach (var file in files)
            {
                var start = DateTime.Now;

                try
                {
                    var relativePath = Path.GetRelativePath(job.SourcePath, file.FullPath);
                    var targetPath = Path.Combine(job.TargetPath, relativePath);

                    _fileService.CopyFile(file.FullPath, targetPath);

                    var duration = (long)(DateTime.Now - start).TotalMilliseconds;

                    _logService.WriteJson(new LogEntry
                    {
                        Timestamp = DateTime.Now,
                        BackupName = job.Name,
                        SourcePath = file.FullPath,
                        TargetPath = targetPath,
                        FileSize = file.Size,
                        TransferTimeMs = duration
                    });

                    _stateService.Update(progress, file, targetPath);
                }
                catch
                {
                    progress.State = BackupJobState.Failed;
                    _stateService.Fail(job.Id);

                    _logService.WriteJson(new LogEntry
                    {
                        Timestamp = DateTime.Now,
                        BackupName = job.Name,
                        SourcePath = file.FullPath,
                        TargetPath = string.Empty,
                        FileSize = file.Size,
                        TransferTimeMs = -1
                    });

                    throw;
                }
            }

            progress.State = BackupJobState.Completed;
            progress.LastUpdate = DateTime.Now;

            _stateService.Complete(job.Id);
        }

        public void ExecuteBackups(IEnumerable<BackupJob> jobs)
        {
            foreach (var job in jobs)
            {
                ExecuteBackup(job);
            }
        }

    }
}
