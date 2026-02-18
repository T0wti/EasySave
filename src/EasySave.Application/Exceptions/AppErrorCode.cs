namespace EasySave.Application.Exceptions
{
    public enum AppErrorCode
    {
        NameEmpty,
        NameTooLong,
        
        SourcePathEmpty,
        SourcePathNotAbsolute,
        SourcePathNotFound,
        
        TargetPathEmpty,
        TargetPathNotAbsolute,
        targetPathNotFound,
        
        SourceEqualsTarget,
        
        JobNotFound,
        JobAlreadyExists,
        
        FileCopyFailed,
        BusinessSoftwareRunning,
        
        ConfigFileCorrupted,
        ConfigFileUnreadable,
        JobsFileCorrupted,
        JobsFileUnreadable,
        
        Unknown
    }
}