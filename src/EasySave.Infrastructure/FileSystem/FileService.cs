using EasySave.Domain.Interfaces;
using EasySave.Domain.Models;

namespace EasySave.Infrastructure.FileSystem
{
    internal class FileService : IFileService
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

        public void CopyFiles(FileInfo[] files, string targetPath)
        {
            var targetDir = Path.GetDirectoryName(targetPath);
            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir!);
            }

            foreach (var file in files)
            {
                file.CopyTo(targetPath, overwrite: true); //true to overwrite files
            }
            //File.Copy(sourcePath, targetPath, overwrite: true);
        }
    }
}