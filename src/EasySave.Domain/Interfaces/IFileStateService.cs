using EasySave.Domain.Models;

namespace EasySave.Domain.Interfaces
{
    // Interface for persisting and retrieving the state of backup jobs to/from storage
    public interface IFileStateService
    {
        /// <summary>
        /// Reads all backup job states from persistent storage.
        /// </summary>
        /// <returns>
        /// A list of <see cref="BackupProgress"/> objects representing the last known
        /// state of each job. Returns an empty list if no state file exists yet.
        /// </returns>
        List<BackupProgress> ReadState();

        /// <summary>
        /// Writes the provided job states to persistent storage,
        /// overwriting the previous state file entirely.
        /// Should be called after each file is processed to keep
        /// the on disk state as fresh as possible.
        /// </summary>
        /// <param name="states">The complete list of job states to persist.</param>
        void WriteState(List<BackupProgress> states);
    }
}