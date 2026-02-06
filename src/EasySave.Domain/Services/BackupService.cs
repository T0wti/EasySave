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

            List<FileDescriptor> files;

            if (job.Type == BackupType.Full)
            {
                files = _fileService.GetFiles(job.SourcePath);
            }
            else
            {
                files = GetDifferentialFiles(job.SourcePath, job.TargetPath);
            }

            var progress = BackupProgress.From(job);

            _stateService.Initialize(progress, files);

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

                    _stateService.Update(progress, file, targetPath);
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

        private string ComputeFileHash(string path)
        {
            using var sha256 = SHA256.Create();
            using var stream = File.OpenRead(path);
            var hashBytes = sha256.ComputeHash(stream);
            return Convert.ToBase64String(hashBytes);
        }

        private List<FileDescriptor> GetDifferentialFiles(string sourceDir, string targetDir)
        {
            var sourceFiles = _fileService.GetFiles(sourceDir);
            var targetFiles = _fileService.GetFiles(targetDir)
                .ToDictionary(f => Path.GetRelativePath(targetDir, f.FullPath));

            var files = new List<FileDescriptor>();

            foreach (var file in sourceFiles)
            {
                var relativePath = Path.GetRelativePath(sourceDir, file.FullPath);

                // Si le fichier n'existe pas dans la target = copier
                if (!targetFiles.TryGetValue(relativePath, out var targetFile))
                {
                    files.Add(file);
                    continue;
                }
                var sourceHash = ComputeFileHash(file.FullPath);
                var targetHash = ComputeFileHash(targetFile.FullPath);

                // Si hash différent = copier
                if (sourceHash != targetHash)
                {
                    files.Add(file);
                }
            }
            return files;
        }
    }
}
