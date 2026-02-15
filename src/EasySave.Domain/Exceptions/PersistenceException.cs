namespace EasySave.Domain.Exceptions
{
    // Thrown when a persistence operation fails: corrupt JSON, inaccessible file, etc...

    public class PersistenceException : EasySaveException
    {
        // The file path that caused the failure, for diagnostic purposes
        public string? FilePath { get; }

        public PersistenceException(string message, string? filePath, Exception innerException)
            : base(message, innerException)
        {
            FilePath = filePath;
        }
    }
}