namespace EasySave.Application.Exceptions
{
    public enum AppErrorCode
    {
        NameEmpty,
        SourcePathEmpty,
        SourcePathNotAbsolute,
        TargetPathEmpty,
        TargetPathNotAbsolute,
        SourceEqualsTarget,
        JobNotFound,
        JobAlreadyExists,
        FileCopyFailed,
        BusinessSoftwareRunning,
        ConfigFileCorrupted,
        ConfigFileUnreadable,
        Unknown
    }
}