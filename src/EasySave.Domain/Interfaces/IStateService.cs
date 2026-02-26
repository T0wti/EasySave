using EasySave.Domain.Models;

namespace EasySave.Domain.Interfaces
{
    // Interface for managing and tracking the state of backup jobs
    public interface IStateService
    {
        /// <summary>
        /// Initialises the state for a backup job before execution begins.
        /// Records the total file count and cumulative size so progress
        /// can be calculated as files are copied.
        /// </summary>
        /// <param name="progress">The progress object for the job being initialised.</param>
        /// <param name="files">The complete list of files to be copied in this run.</param>
        void Initialize(BackupProgress progress, List<FileDescriptor> files);

        /// <summary>
        /// Updates the state after a single file has been copied.
        /// Increments the processed file counter, records the last copied file path,
        /// and persists the new progress to disk.
        /// </summary>
        /// <param name="progress">The progress object for the running job.</param>
        /// <param name="file">Descriptor of the file that was just copied.</param>
        /// <param name="targetPath">Destination path where the file was written.</param>
        void Update(BackupProgress progress, FileDescriptor file, string targetPath);

        /// <summary>
        /// Marks the job as paused.
        /// </summary>
        /// <param name="backupJobId">Unique identifier of the job to pause.</param>
        void Pause(int backupJobId);

        /// <summary>
        /// Marks the job as stopped by user request.
        /// </summary>
        /// <param name="backupJobId">Unique identifier of the job to stop.</param>
        void Stop(int backupJobId);

        /// <summary>
        /// Marks the job as successfully completed and clears its active progress entry.
        /// </summary>
        /// <param name="backupJobId">Unique identifier of the job that finished.</param>
        void Complete(int backupJobId);

        /// <summary>
        /// Marks the job as failed due to an unrecoverable error during execution
        /// (ex : a file copy exception). Progress is preserved for diagnostic purposes.
        /// </summary>
        /// <param name="backupJobId">Unique identifier of the job that failed.</param>
        void Fail(int backupJobId);

        /// <summary>
        /// Marks the job as interrupted by an external condition
        /// </summary>
        /// <param name="backupJobId">Unique identifier of the job that was interrupted.</param>
        void Interrupt(int backupJobId);

        /// <summary>
        /// Marks the job as being in the file comparison phase,
        /// used by differential backup to signal that the strategy
        /// is currently evaluating which files need copying.
        /// </summary>
        /// <param name="backupJobId">Unique identifier of the job being compared.</param>
        void Compare(int backupJobId);

    }

}