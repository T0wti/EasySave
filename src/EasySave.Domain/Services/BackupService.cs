using EasySave.Domain.Enums;
using EasySave.Domain.Interfaces;
using EasySave.Domain.Models;
using EasySave.EasyLog;
using EasySave.EasyLog.Interfaces;

namespace EasySave.Domain.Services
{
    // Service responsible for executing backup jobs
    public class BackupService : IBackupService
    {
        private readonly IFileService _fileService;
        private readonly ILogService _logService;
        private readonly IStateService _stateService;
        private readonly IBackupStrategy _fullStrategy;
        private readonly IBackupStrategy _differentialStrategy;
        private readonly IFileBackupService _fileBackupService;

        private List<BackupJob> _backupJobs;

        // Constructor injects required services and initializes backup jobs list
        public BackupService(
            IFileService fileService,
            IBackupStrategy fullStrategy,
            IBackupStrategy differentialStrategy,
            IFileBackupService fileBackupService,
            IStateService stateService,     
            ILogService logService)
        {
            _fileService = fileService;
            _fullStrategy = fullStrategy;
            _differentialStrategy = differentialStrategy;
            _fileBackupService = fileBackupService;
            _stateService = stateService;
            _logService = logService;

            _backupJobs = _fileBackupService.LoadJobs();
        }

        // Executes a single backup job
        public void ExecuteBackup(BackupJob job)
        {
            // Select backup strategy based on job type
            IBackupStrategy strategy = job.Type == BackupType.Full
            ? _fullStrategy
            : _differentialStrategy;

            var files = strategy.GetFilesToCopy(job.SourcePath, job.TargetPath);

            var progress = BackupProgress.From(job);

            _stateService.Initialize(progress, files);

            // Copy files one by one
            foreach (var file in files)
            {
                var start = DateTime.Now;

                try
                {
                    // Preserve relative paths inside the target folder
                    var relativePath = Path.GetRelativePath(job.SourcePath, file.FullPath);
                    var targetPath = Path.Combine(job.TargetPath, relativePath);

                    // Convert paths to UNC format for logging/network paths
                    string uncSourcePath = PathHelper.ToUncPath(file.FullPath);
                    string uncTargetPath = PathHelper.ToUncPath(targetPath);

                    _fileService.CopyFile(file.FullPath, targetPath);

                    var duration = (long)(DateTime.Now - start).TotalMilliseconds;

                    _logService.Write(new LogEntry
                    {
                        Timestamp = DateTime.Now,
                        BackupName = job.Name,
                        SourcePath = uncSourcePath,
                        TargetPath = uncTargetPath,
                        FileSize = file.Size,
                        TransferTimeMs = duration
                    });

                    _stateService.Update(progress, file, targetPath);
                }
                catch
                {
                    // Handle copy failure: mark job as failed and log
                    progress.State = BackupJobState.Failed;
                    _stateService.Fail(job.Id);

                    _logService.Write(new LogEntry
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

            // Mark backup as completed
            progress.State = BackupJobState.Completed;
            progress.LastUpdate = DateTime.Now;

            _stateService.Complete(job.Id);
        }

        // Executes multiple backup jobs sequentially
        public void ExecuteBackups(IEnumerable<BackupJob> jobs)
        {
            foreach (var job in jobs)
            {
                ExecuteBackup(job);
            }
        }

    }
}
