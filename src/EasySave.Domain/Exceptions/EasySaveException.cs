namespace EasySave.Domain.Exceptions
{
    // Base class for all EasySave domain exceptions
    public class EasySaveException : Exception
    {
        public EasySaveException(string message)
            : base(message) { }

        public EasySaveException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}