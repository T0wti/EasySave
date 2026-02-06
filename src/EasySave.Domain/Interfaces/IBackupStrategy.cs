using EasySave.Domain.Models;

namespace EasySave.Domain.Interfaces
{
    public interface IBackupStrategy
    {
        List<FileDescriptor> GetFilesToCopy(string sourceDir, string targetDir);
    }
}