using EasySave.Domain.Models;

namespace EasySave.Domain.Interfaces
{
    // Interface for executing backup jobs
    public interface IBackupService
    {

        /// <summary>
        /// Executes a single backup job asynchronously.
        /// The caller is responsible for creating the <paramref name="handle"/> beforehand
        /// and registering it in the registry if concurrent control is needed.
        /// The handle is used throughout execution to check for pause and cancellation signals.
        /// </summary>
        /// <param name="job">The backup job definition to execute.</param>
        /// <param name="handle">
        /// Live control handle for this job. The executor will call
        /// <see cref="IBackupJobHandle.WaitIfPaused"/> at each file checkpoint and
        /// observe <see cref="IBackupJobHandle.CancellationToken"/> to support cooperative stopping.
        /// </param>
        Task ExecuteBackup(BackupJob job, IBackupJobHandle handle);

        /// <summary>
        /// Executes multiple backup jobs, potentially in parallel.
        /// Each job gets its own handle, created and tracked via the <paramref name="registry"/>,
        /// allowing individual jobs to be paused, resumed, or stopped independently
        /// while the batch is running.
        /// </summary>
        /// <param name="jobs">The collection of backup jobs to execute.</param>
        /// <param name="registry">
        /// Registry used to create and store a <see cref="IBackupJobHandle"/> per job,
        /// making each handle reachable by external controllers (e.g. the UI or a remote client).
        /// </param>
        Task ExecuteBackups(IEnumerable<BackupJob> jobs, IBackupHandleRegistry registry);
    }
}