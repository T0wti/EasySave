using EasySave.Domain.Enums;

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
