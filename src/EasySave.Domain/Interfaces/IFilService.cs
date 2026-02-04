using EasySave.Domain.Models;

namespace EasySave.Domain.Interfaces;

public interface IFileService
{
    IEnumerable<FileDescriptor> GetFiles(string sourceDirectory);
    void CopyFile(string sourcePath, string targetPath);
}