using EasySave.Domain.Models;

namespace EasySave.Domain.Interfaces
{
    public interface IStateService
    {
        void Initialize(BackupProgress progress, List<FileDescriptor> files);
        void Update(BackupProgress progress, FileDescriptor file, string targetPath);
        void Complete(int backupJobId);
        void Fail(int backupJobId);
    }

}