using EasySave.Domain.Enums;

// Thrown when the external CryptoSoft process exits with a non-zero code
public class CryptoSoftException : EasySaveException
{
    public long ExitCode { get; }
    public CryptoSoftException(string filePath, long exitCode)
        : base(EasySaveErrorCode.CryptoSoftFailed)
    {
        ExitCode = exitCode;
    }
}