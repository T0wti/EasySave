namespace EasySave.Domain.Exceptions
{
    // Base class for all exceptions related to backup job management (create, edit, delete, execute)
    public class BackupJobException : EasySaveException
    {
        public BackupJobException(string message)
            : base(message) { }

        public BackupJobException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    // Thrown when a backup job cannot be found by its ID
    public class BackupJobNotFoundException : BackupJobException
    {
        public int JobId { get; }

        public BackupJobNotFoundException(int jobId)
            : base($"No backup job found with ID '{jobId}'.")
        {
            JobId = jobId;
        }
    }

    // Thrown when trying to create or rename a job with a name that already exists
    public class BackupJobAlreadyExistsException : BackupJobException
    {
        public string JobName { get; }

        public BackupJobAlreadyExistsException(string jobName)
            : base($"A backup job named '{jobName}' already exists.")
        {
            JobName = jobName;
        }
    }

    // Thrown when the maximum number of allowed backup jobs has been reached
    public class BackupJobLimitReachedException : BackupJobException
    {
        public int Limit { get; }

        public BackupJobLimitReachedException(int limit)
            : base($"Cannot create more than {limit} backup jobs.")
        {
            Limit = limit;
        }
    }
}