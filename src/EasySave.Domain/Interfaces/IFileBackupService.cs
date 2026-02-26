using EasySave.Domain.Models;

namespace EasySave.Domain.Interfaces
{
    // Interface for managing persistent storage of backup job configurations
    public interface IFileBackupService
    {
        /// <summary>
        /// Loads all backup job definitions from persistent storage.
        /// </summary>
        /// <returns>
        /// A list of <see cref="BackupJob"/> objects representing the saved configurations.
        /// Returns an empty list if no jobs have been saved yet.
        /// </returns>
        List<BackupJob> LoadJobs();

        /// <summary>
        /// Persists the provided list of backup job definitions to storage,
        /// replacing any previously saved jobs entirely.
        /// </summary>
        /// <param name="jobs">The complete list of jobs to save.</param>
        void SaveJobs(List<BackupJob> jobs);
    }
}
