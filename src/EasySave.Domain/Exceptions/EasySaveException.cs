using EasySave.Domain.Enums;

// Thrown when a daily log is not correctly created
public abstract class EasySaveException : Exception
{
    public EasySaveErrorCode ErrorCode { get; }

    protected EasySaveException(EasySaveErrorCode errorCode, Exception? innerException = null)
        : base(errorCode.ToString(), innerException)
    {
        ErrorCode = errorCode;
    }
}
