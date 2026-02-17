using EasySave.Domain.Enums;

public abstract class EasySaveException : Exception
{
    public EasySaveErrorCode ErrorCode { get; }

    protected EasySaveException(EasySaveErrorCode errorCode, Exception? innerException = null)
        : base(errorCode.ToString(), innerException)
    {
        ErrorCode = errorCode;
    }
}
