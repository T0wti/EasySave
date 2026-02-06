using EasySave.Domain.Interfaces;
using EasySave.Domain.Models;


namespace EasySave.Domain.Services
{
    public class FullBackupStrategy : IBackupStrategy
    {
        private readonly IFileService _fileService;
        public FullBackupStrategy(IFileService fileService)
        {
            _fileService = fileService;
        }

        public List<FileDescriptor> GetFilesToCopy(string sourceDir, string targetDir)
        {
            return _fileService.GetFiles(sourceDir);
        }
    }
}