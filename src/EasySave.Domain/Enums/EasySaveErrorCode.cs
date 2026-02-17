namespace EasySave.Domain.Enums
{
    public enum EasySaveErrorCode
    {
        // Validation
        NameEmpty,
        NameTooLong,
        SourcePathEmpty,
        SourcePathNotAbsolute,
        SourcePathNotFound,
        TargetPathEmpty,
        TargetPathNotAbsolute,
        TargetPathNotFound,
        SourceEqualsTarget,

        // Backup Job Management
        JobNotFound,
        JobAlreadyExists,
        JobLimitReached,

        // Backup Execution
        FileCopyFailed,
        BusinessSoftwareRunning,

        // Persistence
        ConfigFileCorrupted,
        ConfigFileUnreadable,
        JobsFileCorrupted,
        JobsFileUnreadable,

        // Generic
        Unknown
    }
}
