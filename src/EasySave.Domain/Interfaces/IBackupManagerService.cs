using EasySave.Domain.Enums;
using EasySave.Domain.Models;

namespace EasySave.Domain.Interfaces
{
    // Interface for managing backup jobs: creation, deletion, editing, retrieval, and execution
    public interface IBackupManagerService
    {
        /// <summary>
        /// Creates a new backup job and persists it.
        /// </summary>
        /// <param name="name">Human-readable name identifying the job.</param>
        /// <param name="source">Full path of the directory or file to back up.</param>
        /// <param name="target">Full path of the destination directory.</param>
        /// <param name="type">Backup strategy to use (Full or Differential).</param>
        void CreateBackupJob(string name, string source, string target, BackupType type);

        /// <summary>
        /// Permanently removes the backup job with the given identifier.
        /// </summary>
        /// <param name="id">Unique identifier of the job to delete.</param>
        void DeleteBackupJob(int id);

        /// <summary>
        /// Updates the definition of an existing backup job.
        /// </summary>
        /// <param name="id">Unique identifier of the job to edit.</param>
        /// <param name="newName">Replacement name for the job.</param>
        /// <param name="newSource">Replacement source path.</param>
        /// <param name="newTarget">Replacement target path.</param>
        /// <param name="newType">Replacement backup strategy.</param>
        void EditBackupJob(int id, string newName, string newSource, string newTarget, BackupType newType);

        /// <summary>
        /// Returns a read-only snapshot of all currently defined backup jobs.
        /// The list reflects the persisted state at the time of the call.
        /// </summary>
        public IReadOnlyList<BackupJob> GetBackupJobs();
    }
}