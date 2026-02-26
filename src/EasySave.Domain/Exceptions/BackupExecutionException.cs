using EasySave.Domain.Enums;

// Thrown when a backup job fails during execution, typically due to a file copy error
public class BackupExecutionException : EasySaveException
{
    public string BackupName { get; }
    public string FailedFilePath { get; }

    public BackupExecutionException(
        string backupName,
        string failedFilePath,
        Exception innerException)
        : base(EasySaveErrorCode.FileCopyFailed, innerException)
    {
        BackupName = backupName;
        FailedFilePath = failedFilePath;
    }
}
