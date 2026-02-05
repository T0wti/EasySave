using EasySave.Domain.Models;

namespace EasySave.Domain.Interfaces
{
    public interface IFileStateService
    {
        List<BackupProgress> ReadState();
        void WriteState(List<BackupProgress> states);
    }
}