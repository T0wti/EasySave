using EasySave.Domain.Enums;

// Thrown when a backup job is created or edited with not correct attributes
public class BackupValidationException : EasySaveException
{
    public string FieldName { get; }

    public BackupValidationException(string fieldName, EasySaveErrorCode errorCode)
        : base(errorCode)
    {
        FieldName = fieldName;
    }
}
