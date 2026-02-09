using EasySave.Domain.Interfaces;
using EasySave.Domain.Models;


namespace EasySave.Domain.Services
{
    // Strategy for performing a full backup: all files from the source directory are copied
    public class FullBackupStrategy : IBackupStrategy
    {
        private readonly IFileService _fileService;
        public FullBackupStrategy(IFileService fileService)
        {
            _fileService = fileService;
        }

        // Returns the list of files to copy for a full backup
        public List<FileDescriptor> GetFilesToCopy(string sourceDir, string targetDir)
        {
            return _fileService.GetFiles(sourceDir);
        }
    }
}