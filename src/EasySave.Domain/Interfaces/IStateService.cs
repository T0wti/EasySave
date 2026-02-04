using EasySave.Domain.Models;

namespace EasySave.Domain.Interfaces
{
    public interface IStateService
    {
        void Initialize(BackupProgress progress);
        void Update(BackupProgress progress);
        void Complete(int backupJobId);
        void Fail(int backupJobId);
    }

}