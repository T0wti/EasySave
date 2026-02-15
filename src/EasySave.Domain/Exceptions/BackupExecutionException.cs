namespace EasySave.Domain.Exceptions
{
    // Thrown when a backup job fails during execution 
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