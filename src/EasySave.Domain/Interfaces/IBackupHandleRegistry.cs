namespace EasySave.Domain.Interfaces
{
    // Thread-safe registry of active backup job handles.
    public interface IBackupHandleRegistry
    {
        /// <summary>
        /// Registers a handle for a backup job that has just started.
        /// </summary>
        /// <param name="jobId">Unique identifier of the backup job.</param>
        /// <param name="handle">Handle exposing control operations for that job.</param>
        void Register(int jobId, IBackupJobHandle handle);

        /// <summary>
        /// Removes the handle of a backup job that has completed or been cancelled.
        /// </summary>
        /// <param name="jobId">Unique identifier of the backup job to deregister.</param>
        void Unregister(int jobId);

        /// <summary>
        /// Returns the handle for a specific job, or <c>null</c> if no active job
        /// with that identifier exists in the registry.
        /// </summary>
        /// <param name="jobId">Unique identifier of the backup job to look up.</param>
        IBackupJobHandle? Get(int jobId);

        /// <summary>
        /// Returns a snapshot of all currently registered job handles,
        /// each paired with its corresponding job identifier.
        /// </summary>
        IEnumerable<(int JobId, IBackupJobHandle Handle)> GetAll();
    }
}