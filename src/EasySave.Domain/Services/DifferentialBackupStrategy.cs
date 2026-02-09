using EasySave.Domain.Interfaces;
using EasySave.Domain.Models;
using System.Security.Cryptography;


namespace EasySave.Domain.Services
{
    // Strategy for performing a differential backup : Only files that are new or changed compared to the target directory are copied
    public class DifferentialBackupStrategy : IBackupStrategy
    {
        private readonly IFileService _fileService;

        // Constructor injects the file service
        public DifferentialBackupStrategy(IFileService fileService)
        {
            _fileService = fileService;
        }

        // Returns the list of files that need to be copied for a differential backup
        public List<FileDescriptor> GetFilesToCopy(string sourceDir, string targetDir)
        {
            var sourceFiles = _fileService.GetFiles(sourceDir);

            // Get all files from the target directory, if it exists, and map by relative path
            var targetFiles = Directory.Exists(targetDir)
                    ? _fileService.GetFiles(targetDir).ToDictionary(f => Path.GetRelativePath(targetDir, f.FullPath))
                    : new Dictionary<string, FileDescriptor>();

            var files = new List<FileDescriptor>();

            // Compare each source file to the corresponding target file
            foreach (var file in sourceFiles)
            {
                var relativePath = Path.GetRelativePath(sourceDir, file.FullPath);

                // If the file does not exist in the target, add it to the copy list
                if (!targetFiles.TryGetValue(relativePath, out var targetFile))
                {
                    files.Add(file);
                    continue;
                }

                // If file sizes differ, it has changed and should be copied
                if (file.Size != targetFile.Size)
                {
                    files.Add(file);
                    continue;
                }

                // If sizes are the same, compare SHA-256 hashes to detect changes
                var sourceHash = ComputeFileHash(file.FullPath);
                var targetHash = ComputeFileHash(targetFile.FullPath);

                if (sourceHash != targetHash)
                    files.Add(file);
            }

            return files;
        }

        // Computes the SHA-256 hash of a file
        private string ComputeFileHash(string path)
        {
            using var sha256 = SHA256.Create();
            using var stream = File.OpenRead(path);
            var hashBytes = sha256.ComputeHash(stream);
            return Convert.ToBase64String(hashBytes);
        }

    }
}