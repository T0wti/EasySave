namespace EasySave.Application.Exceptions
{
    // Enums for application for the mappinf of the exceptions errors
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