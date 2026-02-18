namespace EasySave.Application.Exceptions
{
    // Exception in the application to transmit in the viewmodels
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