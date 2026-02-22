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
        private readonly IPriorityGate _priorityGate;
        private readonly IEnumerable<string> _priorityExtensions;

        public BackupService(
            IFileService fileService,
            IBackupStrategy fullStrategy,
            IBackupStrategy differentialStrategy,
            IStateService stateService,
            ILogService logService,
            IBusinessSoftwareWatcher watcher,
            ICryptoSoftService cryptoSoftService,
            IPriorityGate priorityGate)
        {
            _fileService = fileService;
            _fullStrategy = fullStrategy;
            _differentialStrategy = differentialStrategy;
            _stateService = stateService;
            _logService = logService;
            _watcher = watcher;
            _cryptoSoftService = cryptoSoftService;
            _priorityGate = priorityGate;   
        }

        // Executes a single backup job asynchronously
        public async Task ExecuteBackup(BackupJob job, IBackupJobHandle handle)
        {
            using var watcherCts = new CancellationTokenSource();
            var watchTask = _watcher.WatchAsync(watcherCts.Token);

            try
            {
                await ExecuteBackupCore(job, handle).ConfigureAwait(false);
            }
            finally
            {
                watcherCts.Cancel();
                await watchTask.ConfigureAwait(false);
            }
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
                        await ExecuteBackupCore(job, handle).ConfigureAwait(false);
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

        // Logic of the backup job
        private async Task ExecuteBackupCore(BackupJob job, IBackupJobHandle handle)
        {
            IBackupStrategy strategy = job.Type == BackupType.Full
                ? _fullStrategy
                : _differentialStrategy;

            var files = await Task.Run(
                () => strategy.GetFilesToCopy(job.SourcePath, job.TargetPath),
                handle.CancellationToken).ConfigureAwait(false); // If the stop is engaged stop the getfile 

            var progress = BackupProgress.From(job);
            _stateService.Initialize(progress, files);

            int priorityCount = files.Count(f => _priorityGate.IsPriority(f.FullPath));
            _priorityGate.RegisterPriorityFiles(priorityCount);

            var orderedFiles = files
            .OrderByDescending(f => _priorityGate.IsPriority(f.FullPath))
            .ToList();

            // Copy files one by one within this job
            foreach (var file in orderedFiles)
            {
                if (handle.IsPaused)
                    _stateService.Pause(job.Id);

                // Pause : waits here between files 
                handle.WaitIfPaused();

                if (!handle.CancellationToken.IsCancellationRequested)
                {
                    progress.State = BackupJobState.Active;
                    progress.LastUpdate = DateTime.Now;
                }

                // Stop: exits the loop
                handle.CancellationToken.ThrowIfCancellationRequested();

                bool isPriority = _priorityGate.IsPriority(file.FullPath);
                await _priorityGate.WaitIfNeededAsync(isPriority, handle.CancellationToken)
                                   .ConfigureAwait(false);

                var start = DateTime.Now;
                try
                {
                    // Preserve relative paths inside the target folder
                    var relativePath = Path.GetRelativePath(job.SourcePath, file.FullPath);
                    var targetPath = Path.Combine(job.TargetPath, relativePath);

                    await Task.Run(() => _fileService.CopyFile(file.FullPath, targetPath))
                              .ConfigureAwait(false);

                    var duration = (long)(DateTime.Now - start).TotalMilliseconds;

                    long encryptionTime = 0;
                    if (_cryptoSoftService.ShouldEncrypt(targetPath))
                        encryptionTime = await Task.Run(() => _cryptoSoftService.Encrypt(targetPath))
                                                   .ConfigureAwait(false);

                    _logService.Write(new LogEntry
                    {
                        Timestamp = DateTime.Now,
                        BackupName = job.Name,
                        // Convert paths to UNC format for logging/network paths
                        SourcePath = PathHelper.ToUncPath(file.FullPath),
                        TargetPath = PathHelper.ToUncPath(targetPath),
                        FileSize = file.Size,
                        TransferTimeMs = duration,
                        EncryptionTimeMs = encryptionTime
                    });

                    _stateService.Update(progress, file, targetPath);

                    if (isPriority)
                        _priorityGate.NotifyPriorityFileCopied();
                }
                catch (OperationCanceledException)
                {
                    if (isPriority)
                        _priorityGate.NotifyPriorityFileCopied();
                    _stateService.Stop(job.Id);
                    throw;
                }
                catch (Exception ex)
                {
                    if (isPriority)
                        _priorityGate.NotifyPriorityFileCopied();
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
    }
}