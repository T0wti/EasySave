using EasySave.Domain.Models;

namespace EasySave.Domain.Interfaces
{

    public interface IFileService
    {
        // Interface for basic file operations used in backups
        List<FileDescriptor> GetFiles(string sourceDirectory);
        void CopyFile(string sourcePath, string targetPath);
    }
}