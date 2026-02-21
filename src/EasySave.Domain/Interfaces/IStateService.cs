using EasySave.Domain.Models;

namespace EasySave.Domain.Interfaces
{
    // Interface for managing and tracking the state of backup jobs
    public interface IStateService
    {
        void Initialize(BackupProgress progress, List<FileDescriptor> files);
        void Update(BackupProgress progress, FileDescriptor file, string targetPath);
        void Pause(int backupJobId);
        void Stop(int backupJobId);
        void Complete(int backupJobId);
        void Fail(int backupJobId);
        void Interrupt(int backupJobId);
    }

}