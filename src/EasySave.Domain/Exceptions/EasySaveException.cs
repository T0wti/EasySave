namespace EasySave.Domain.Exceptions
{
    // Base class for all EasySave domain exceptions.
    // Catching this type handles any business error from the domain layer.
    public class EasySaveException : Exception
    {
        public EasySaveException(string message)
            : base(message) { }

        public EasySaveException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}