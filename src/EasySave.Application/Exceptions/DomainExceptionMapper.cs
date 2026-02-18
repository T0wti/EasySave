using EasySave.Domain.Enums;

namespace EasySave.Application.Exceptions
{
    public static class DomainExceptionMapper
    {
        public static AppException Map(EasySaveException ex)
        {
            var code = ex.ErrorCode switch
            {
                EasySaveErrorCode.NameEmpty => AppErrorCode.NameEmpty,
                EasySaveErrorCode.SourcePathEmpty => AppErrorCode.SourcePathEmpty,
                EasySaveErrorCode.SourcePathNotAbsolute => AppErrorCode.SourcePathNotAbsolute,
                EasySaveErrorCode.TargetPathEmpty => AppErrorCode.TargetPathEmpty,
                EasySaveErrorCode.TargetPathNotAbsolute => AppErrorCode.TargetPathNotAbsolute,
                EasySaveErrorCode.SourceEqualsTarget => AppErrorCode.SourceEqualsTarget,
                EasySaveErrorCode.JobNotFound => AppErrorCode.JobNotFound,
                EasySaveErrorCode.JobAlreadyExists => AppErrorCode.JobAlreadyExists,
                EasySaveErrorCode.FileCopyFailed => AppErrorCode.FileCopyFailed,
                EasySaveErrorCode.BusinessSoftwareRunning => AppErrorCode.BusinessSoftwareRunning,
                EasySaveErrorCode.ConfigFileCorrupted => AppErrorCode.ConfigFileCorrupted,
                EasySaveErrorCode.ConfigFileUnreadable => AppErrorCode.ConfigFileUnreadable,
                _ => AppErrorCode.Unknown
            };

            var fieldName = (ex as BackupValidationException)?.FieldName;
            return new AppException(code, fieldName);
        }
    }
}