using EasySave.Domain.Models;

namespace EasySave.Domain.Interfaces;

public interface IFileService
{
    List<FileDescriptor> GetFiles(string sourceDirectory);
    void CopyFile(string sourcePath, string targetPath);
}