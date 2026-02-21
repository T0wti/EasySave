namespace EasySave.Domain.Enums
{
    // Enum representing the current state of a backup job
    public enum BackupJobState
    {
        Inactive,
        Active,
        Paused,
        Completed,
        Failed,
        Stopped,
        Interrupted
    }
}
