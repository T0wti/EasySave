using EasySave.Domain.Enums;

namespace EasySave.Domain.Exceptions
{
    // Thrown when a persistence operation fails: corrupt JSON, inaccessible file, etc...

    public class PersistenceException : EasySaveException
    {
        public string? FilePath { get; }

        public PersistenceException(
            EasySaveErrorCode errorCode,
            string? filePath,
            Exception innerException)
            : base(errorCode, innerException)
        {
            FilePath = filePath;
        }
    }

}