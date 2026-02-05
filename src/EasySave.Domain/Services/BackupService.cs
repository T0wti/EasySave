using EasySave.Domain.Enums;
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
   
        public BackupService(
        IFileService fileService,
        ILogService logService,
        IStateService stateService)
        {
            _fileService = fileService;
            _logService = logService;
            _stateService = stateService;
        }

        public void ExecuteBackup(BackupJob job)
        {

            var files = _fileService.GetFiles(job.SourcePath).ToList();

            var progress = BackupProgress.From(job);
            progress.TotalFiles = files.Count;
            progress.TotalSize = files.Sum(f => f.Size);
            progress.RemainingFiles = progress.TotalFiles;
            progress.RemainingSize = progress.TotalSize;

            _stateService.Initialize(progress);

            foreach (var file in files)
            {
                var start = DateTime.Now;

                try
                {
                    var relativePath = Path.GetRelativePath(
                        job.SourcePath,
                        file.FullPath
                    );

                    var targetPath = Path.Combine(
                        job.TargetPath,
                        relativePath
                    );

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

                    progress.RemainingFiles--;
                    progress.RemainingSize -= file.Size;
                    progress.CurrentSourceFile = file.FullPath;
                    progress.CurrentTargetFile = targetPath;
                    progress.LastUpdate = DateTime.Now;

                    _stateService.Update(progress);
                }
                catch (Exception ex)
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