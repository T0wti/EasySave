using EasySave.Domain.Interfaces;
using EasySave.Domain.Models;

namespace EasySave.Domain.Services
{
    public class FileService : IFileService
    {
        public IEnumerable<FileDescriptor> GetFiles(string sourceDirectory)
        {
            var files = Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                var info = new FileInfo(file);
                yield return new FileDescriptor
                {
                    FullPath = info.FullName,
                    Size = info.Length
                };
            }
        }

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
