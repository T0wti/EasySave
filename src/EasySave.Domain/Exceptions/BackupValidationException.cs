using EasySave.Domain.Enums;

public class BackupValidationException : EasySaveException
{
    public string FieldName { get; }

    public BackupValidationException(string fieldName, EasySaveErrorCode errorCode)
        : base(errorCode)
    {
        FieldName = fieldName;
    }
}
