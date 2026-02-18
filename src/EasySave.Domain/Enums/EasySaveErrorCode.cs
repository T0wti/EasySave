namespace EasySave.Domain.Enums
{
    // Enum representing the type of errors
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
