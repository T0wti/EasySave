// EasySave.Domain/Exceptions/BackupExceptions.cs
namespace EasySave.Domain.Exceptions
{
    public class BackupException : Exception
    {
        public BackupException(string message) : base(message) { }
        public BackupException(string message, Exception inner) : base(message, inner) { }
    }

    public class JobNotFoundException : BackupException
    {
        public int JobId { get; }
        public JobNotFoundException(int jobId)
            : base($"Backup job with ID {jobId} not found.")
        {
            JobId = jobId;
        }
    }

    public class DuplicateJobNameException : BackupException
    {
        public string JobName { get; }
        public DuplicateJobNameException(string name)
            : base($"A backup job named '{name}' already exists.")
        {
            JobName = name;
        }
    }

    public class InvalidPathException : BackupException
    {
        public string ParamName { get; }
        public string Path { get; }

        public InvalidPathException(string paramName, string path)
            : base($"{paramName} path '{path}' is invalid or not rooted.")
        {
            ParamName = paramName;
            Path = path;
        }
    }

    public class MaxJobsExceededException : BackupException
    {
        public int MaxJobs { get; }
        public MaxJobsExceededException(int maxJobs)
            : base($"Cannot create more than {maxJobs} backup jobs.")
        {
            MaxJobs = maxJobs;
        }
    }

    public class BackupExecutionException : BackupException
    {
        public BackupExecutionException(string message) : base(message) { }
        public BackupExecutionException(string message, Exception inner) : base(message, inner) { }
    }

    public class StateInconsistencyException : BackupException
    {
        public StateInconsistencyException(string message) : base(message) { }
    }
}