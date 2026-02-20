using EasySave.Domain.Interfaces;
using System.Collections.Concurrent;

namespace EasySave.Domain.Services
{
    // Thread safe registry of active backup job handles
    public class BackupHandleRegistry : IBackupHandleRegistry
    {
        private readonly ConcurrentDictionary<int, IBackupJobHandle> _handles = new(); // Dictionary thread safe

        public void Register(int jobId, IBackupJobHandle handle) //Add an entry with the key (id of jobs) and the value (handle)
            => _handles[jobId] = handle;

        public void Unregister(int jobId) // Reverse of register (scilicet) 
        {
            if (_handles.TryRemove(jobId, out var handle))
                handle.Dispose(); // Free the memory 
        }

        public IBackupJobHandle? Get(int jobId) // Return the handle of a job
            => _handles.TryGetValue(jobId, out var handle) ? handle : null;

        public IEnumerable<(int JobId, IBackupJobHandle Handle)> GetAll() // Return a list with all the active jobs
            => _handles.Select(kv => (kv.Key, kv.Value));
    }
}