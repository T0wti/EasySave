namespace EasySave.Domain.Interfaces
{
    // Thread-safe registry of active backup job handles.
    public interface IBackupHandleRegistry
    {
        void Register(int jobId, IBackupJobHandle handle);
        void Unregister(int jobId);

        IBackupJobHandle? Get(int jobId);

        IEnumerable<(int JobId, IBackupJobHandle Handle)> GetAll();
    }
}