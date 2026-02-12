namespace EasySave.Domain.Exceptions
{
    // Thrown when a backup job fails during execution (e.g. a file could not be copied).
    // Always wraps the original exception as InnerException to preserve the full error context.
    public class BackupExecutionException : EasySaveException
    {
        public string BackupName { get; }
        public string? FailedFilePath { get; }

        public BackupExecutionException(string backupName, string failedFilePath, Exception innerException)
            : base($"Backup '{backupName}' failed while copying '{failedFilePath}'.", innerException)
        {
            BackupName = backupName;
            FailedFilePath = failedFilePath;
        }
    }
}