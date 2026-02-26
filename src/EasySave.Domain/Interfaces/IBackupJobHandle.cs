namespace EasySave.Domain.Interfaces
{
    // Controls the execution flow of a single running backup job

    public interface IBackupJobHandle : IDisposable
    {
        /// <summary>
        /// Indicates whether the job is currently suspended.
        /// </summary>
        bool IsPaused { get; }

        /// <summary>
        /// Blocks the calling thread if the job has been paused,
        /// and returns once it is resumed or cancelled.
        /// Should be called at regular checkpoints inside the backup loop.
        /// </summary>
        void WaitIfPaused();

        /// <summary>
        /// Suspends execution of the backup job.
        /// The job remains in memory and can be resumed at any time.
        /// </summary>
        void Pause();

        /// <summary>
        /// Resumes a previously paused job, unblocking any thread
        /// waiting inside <see cref="WaitIfPaused"/>.
        /// </summary>
        void Resume();

        /// <summary>
        /// Requests cancellation of the backup job by triggering
        /// <see cref="CancellationToken"/>. The executing thread is
        /// expected to observe the token and terminate gracefully.
        /// </summary>
        void Stop();

        CancellationToken CancellationToken { get; }
    }
}