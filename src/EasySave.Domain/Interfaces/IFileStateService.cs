using EasySave.Domain.Models;

namespace EasySave.Domain.Interfaces
{
    // Interface for persisting and retrieving the state of backup jobs to/from storage
    public interface IFileStateService
    {
        List<BackupProgress> ReadState();
        void WriteState(List<BackupProgress> states);
    }
}