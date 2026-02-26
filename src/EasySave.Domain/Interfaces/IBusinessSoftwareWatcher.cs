namespace EasySave.Domain.Interfaces
{
    //Monitors the business software in the background during backup execution
    //Automatically pauses all jobs when the software is detected, resumes when it stops
    public interface IBusinessSoftwareWatcher
    {
        /// <summary>
        /// Starts the background monitoring loop asynchronously.
        /// The loop polls <see cref="IBusinessSoftwareService.IsBusinessSoftwareRunning"/>
        /// at a regular interval and calls <see cref="IBackupJobHandle.Pause"/> or
        /// <see cref="IBackupJobHandle.Resume"/> on all handles in the registry accordingly.
        /// </summary>
        /// <param name="stopWhen">
        /// Token that signals the watcher to stop polling and exit cleanly.
        /// Should be cancelled when the last backup job completes or when the
        /// application is shutting down.
        /// </param>
        Task WatchAsync(CancellationToken stopWhen);
    }
}