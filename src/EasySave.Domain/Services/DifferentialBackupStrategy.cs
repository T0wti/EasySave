using EasySave.Domain.Interfaces;
using EasySave.Domain.Models;
using System.Security.Cryptography;


namespace EasySave.Domain.Services
{
    public class DifferentialBackupStrategy : IBackupStrategy
    {
        private readonly IFileService _fileService;

        public DifferentialBackupStrategy(IFileService fileService)
        {
            _fileService = fileService;
        }

        public List<FileDescriptor> GetFilesToCopy(string sourceDir, string targetDir)
        {
            var sourceFiles = _fileService.GetFiles(sourceDir);
            var targetFiles = Directory.Exists(targetDir)
                    ? _fileService.GetFiles(targetDir).ToDictionary(f => Path.GetRelativePath(targetDir, f.FullPath))
                    : new Dictionary<string, FileDescriptor>();

            var files = new List<FileDescriptor>();

            foreach (var file in sourceFiles)
            {
                var relativePath = Path.GetRelativePath(sourceDir, file.FullPath);

                if (!targetFiles.TryGetValue(relativePath, out var targetFile))
                {
                    files.Add(file);
                    continue;
                }

                if (file.Size != targetFile.Size)
                {
                    files.Add(file);
                    continue;
                }

                var sourceHash = ComputeFileHash(file.FullPath);
                var targetHash = ComputeFileHash(targetFile.FullPath);

                if (sourceHash != targetHash)
                    files.Add(file);
            }

            return files;
        }
        private string ComputeFileHash(string path)
        {
            using var sha256 = SHA256.Create();
            using var stream = File.OpenRead(path);
            var hashBytes = sha256.ComputeHash(stream);
            return Convert.ToBase64String(hashBytes);
        }

    }
}