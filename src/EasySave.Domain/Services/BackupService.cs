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
        private readonly IBusinessSoftwareService _businessSoftwareService;
        private readonly ICryptoSoftService _cryptoSoftService;

        public BackupService(
            IFileService fileService,
            IBackupStrategy fullStrategy,
            IBackupStrategy differentialStrategy,
            IStateService stateService,
            ILogService logService,
            IBusinessSoftwareService businessSoftwareService,
            ICryptoSoftService cryptoSoftService)
        {
            _fileService = fileService;
            _fullStrategy = fullStrategy;
            _differentialStrategy = differentialStrategy;
            _stateService = stateService;
            _logService = logService;
            _businessSoftwareService = businessSoftwareService;
            _cryptoSoftService = cryptoSoftService;
        }

        // Executes a single backup job asynchronously
        public async Task ExecuteBackupAsync(BackupJob job)
        {    

            IBackupStrategy strategy = job.Type == BackupType.Full
                ? _fullStrategy
                : _differentialStrategy;

            var files = await Task.Run(() => strategy.GetFilesToCopy(job.SourcePath, job.TargetPath));

            var progress = BackupProgress.From(job);
            _stateService.Initialize(progress, files);
            
            if (_businessSoftwareService.IsBusinessSoftwareRunning())
                throw new BusinessSoftwareRunningException(_businessSoftwareService.GetConfiguredName());

            // Copy files one by one within this job
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
        public async Task ExecuteBackupsAsync(IEnumerable<BackupJob> jobs)
        {
            if (_businessSoftwareService.IsBusinessSoftwareRunning())
                throw new BusinessSoftwareRunningException(_businessSoftwareService.GetConfiguredName());

            var tasks = jobs.Select(job => ExecuteBackupAsync(job));

            await Task.WhenAll(tasks);
        }
    }
}