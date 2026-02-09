using EasySave.Domain.Interfaces;
using EasySave.Domain.Models;

namespace EasySave.Domain.Services
{
    // Service responsible for file operations such as listing and copying files
    public class FileService : IFileService
    {
        // Retrieves all files from the source directory and its subdirectories
        public List<FileDescriptor> GetFiles(string sourceDirectory)
        {
            return Directory
                    .GetFiles(sourceDirectory, "*", SearchOption.AllDirectories)
                    .Select(f =>
                    {
                        var info = new FileInfo(f);
                        return new FileDescriptor
                        {
                            FullPath = info.FullName,
                            Size = info.Length
                        };
                    })
                    .ToList();
        }

        // Copies a file from sourcePath to targetPath, creating directories if needed
        public void CopyFile(string sourcePath, string targetPath)
        {
            var targetDir = Path.GetDirectoryName(targetPath);
            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir!);
            }

            File.Copy(sourcePath, targetPath, overwrite: true);
        }
    }
}

