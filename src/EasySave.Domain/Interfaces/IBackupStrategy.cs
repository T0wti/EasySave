using EasySave.Domain.Models;

namespace EasySave.Domain.Interfaces
{
    // Interface for a backup strategy, defining how to determine which files should be copied
    public interface IBackupStrategy
    {
        List<FileDescriptor> GetFilesToCopy(string sourceDir, string targetDir);
    }
}