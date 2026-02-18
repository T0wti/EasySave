namespace EasySave.Application.Exceptions
{
    public class AppException : Exception
    {
        public AppErrorCode ErrorCode { get; }
        public string? FieldName { get; }

        public AppException(AppErrorCode errorCode, string? fieldName = null)
            : base(errorCode.ToString())
        {
            ErrorCode = errorCode;
            FieldName = fieldName;
        }
    }
}