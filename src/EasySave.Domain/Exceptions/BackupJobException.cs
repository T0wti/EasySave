using EasySave.Domain.Enums;

namespace EasySave.Domain.Exceptions
{
    // Base class for all exceptions related to backup job management (create, edit, delete, execute)
    public abstract class BackupJobException : EasySaveException
    {
        protected BackupJobException(EasySaveErrorCode errorCode)
            : base(errorCode) { }
    }


    // Thrown when a backup job cannot be found by its ID
    public class BackupJobNotFoundException : BackupJobException
    {
        public int JobId { get; }

        public BackupJobNotFoundException(int jobId)
            : base(EasySaveErrorCode.JobNotFound)
        {
            JobId = jobId;
        }
    }


    // Thrown when trying to create or rename a job with a name that already exists
    public class BackupJobAlreadyExistsException : BackupJobException
    {
        public string JobName { get; }

        public BackupJobAlreadyExistsException(string jobName)
            : base(EasySaveErrorCode.JobAlreadyExists)
        {
            JobName = jobName;
        }
    }

}