namespace EasySave.Domain.Exceptions
{
    // Thrown when user-provided input fails validation before a job can be created or edited.
    // Typically used for invalid source/target path formats.
    public class BackupValidationException : EasySaveException
    {
        // The name of the field that failed validation, for display purposes.
        public string FieldName { get; }

        public BackupValidationException(string fieldName, string message)
            : base(message)
        {
            FieldName = fieldName;
        }
    }
}