using EasySave.Domain.Enums;

public class CryptoSoftException : EasySaveException
{
    public long ExitCode { get; }
    public CryptoSoftException(string filePath, long exitCode)
        : base(EasySaveErrorCode.CryptoSoftFailed)
    {
        ExitCode = exitCode;
    }
}