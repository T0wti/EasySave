using EasySave.Domain.Enums;
using EasySave.Domain.Interfaces;
using EasySave.Domain.Models;
using EasySave.EasyLog.Interfaces;
using System.Diagnostics;
using System.Security.Cryptography;

namespace EasySave.Domain.Services
{
    public class BackupService : IBackupService
    {
        private readonly IFileService _fileService;
        private readonly ILogService _logService;
        private readonly IStateService _stateService;
        private readonly IBackupStrategy _fullStrategy;
        private readonly IBackupStrategy _differentialStrategy;


        public BackupService(
        IFileService fileService,
        ILogService logService,
        IStateService stateService,
        IBackupStrategy fullStrategy,
        IBackupStrategy differentialStrategy)
        {
            _fileService = fileService;
            _logService = logService;
            _stateService = stateService;
            _fullStrategy = fullStrategy;
            _differentialStrategy = differentialStrategy;
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
