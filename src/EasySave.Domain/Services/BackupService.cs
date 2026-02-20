using EasySave.Domain.Enums;
using EasySave.Domain.Exceptions;
using EasySave.Domain.Helpers;
using EasySave.Domain.Interfaces;
using EasySave.Domain.Models;
using EasySave.EasyLog.Interfaces;

namespace EasySave.Domain.Services
{
    public class BackupService : IBackupService
    {
        private readonly IFileService _fileService;
        private readonly ILogService _logService;
        private readonly IStateService _stateService;
        private readonly IBackupStrategy _fullStrategy;
        private readonly IBackupStrategy _differentialStrategy;
        private readonly IBusinessSoftwareWatcher _watcher;
        private readonly ICryptoSoftService _cryptoSoftService;

        public BackupService(
            IFileService fileService,
            IBackupStrategy fullStrategy,
            IBackupStrategy differentialStrategy,
            IStateService stateService,
            ILogService logService,
            IBusinessSoftwareWatcher watcher,
            ICryptoSoftService cryptoSoftService)
        {
            _fileService = fileService;
            _fullStrategy = fullStrategy;
            _differentialStrategy = differentialStrategy;
            _stateService = stateService;
            _logService = logService;
            _watcher = watcher;
            _cryptoSoftService = cryptoSoftService;
        }

        // Executes a single backup job asynchronously
        public async Task ExecuteBackup(BackupJob job, IBackupJobHandle handle)
        {

            IBackupStrategy strategy = job.Type == BackupType.Full
                ? _fullStrategy
                : _differentialStrategy;

            var files = await Task.Run(
                () => strategy.GetFilesToCopy(job.SourcePath, job.TargetPath),
                handle.CancellationToken); // If the stop is engaged stop the getfile 

            var progress = BackupProgress.From(job);
            _stateService.Initialize(progress, files);

            // Copy files one by one within this job
            foreach (var file in files)
            {

                // Pause: waits here between files 
                handle.WaitIfPaused();

                // Stop: exits the loop
                handle.CancellationToken.ThrowIfCancellationRequested();

                var start = DateTime.Now;

                try
                {
                    // Preserve relative paths inside the target folder
                    var relativePath = Path.GetRelativePath(job.SourcePath, file.FullPath);
                    var targetPath = Path.Combine(job.TargetPath, relativePath);

                    // Convert paths to UNC format for logging/network paths
                    string uncSourcePath = PathHelper.ToUncPath(file.FullPath);
                    string uncTargetPath = PathHelper.ToUncPath(targetPath);

                    await Task.Run(() => _fileService.CopyFile(file.FullPath, targetPath));

                    var duration = (long)(DateTime.Now - start).TotalMilliseconds;

                    long encryptionTime = 0;
                    if (_cryptoSoftService.ShouldEncrypt(targetPath))
                        encryptionTime = await Task.Run(() => _cryptoSoftService.Encrypt(targetPath));

                    _logService.Write(new LogEntry
                    {
                        Timestamp = DateTime.Now,
                        BackupName = job.Name,
                        SourcePath = uncSourcePath,
                        TargetPath = uncTargetPath,
                        FileSize = file.Size,
                        TransferTimeMs = duration,
                        EncryptionTimeMs = encryptionTime
                    });

                    _stateService.Update(progress, file, targetPath);
                }
                catch (Exception ex)
                {
                    // Handle copy failure: mark job as failed and log
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

                    throw new BackupExecutionException(job.Name, file.FullPath, ex);
                }
            }

            // Mark backup as completed
            progress.State = BackupJobState.Completed;
            progress.LastUpdate = DateTime.Now;
            _stateService.Complete(job.Id);
        }

        // Executes multiple backup jobs in parallel using Task.WhenAll
        public async Task ExecuteBackups(IEnumerable<BackupJob> jobs, IBackupHandleRegistry registry)
        {
            using var watcherCts = new CancellationTokenSource();

            var watchTask = _watcher.WatchAsync(watcherCts.Token);

            try
            {
                var tasks = jobs.Select(async job =>
                {
                    var handle = new BackupJobHandle();
                    registry.Register(job.Id, handle);
                    try
                    {
                        await ExecuteBackup(job, handle).ConfigureAwait(false);
                    }
                    finally
                    {
                        registry.Unregister(job.Id);
                    }
                });
                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
            finally
            {
                watcherCts.Cancel();
                await watchTask.ConfigureAwait(false);
            }
        }
    }
}