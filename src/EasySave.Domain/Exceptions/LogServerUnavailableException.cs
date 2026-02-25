using EasySave.Domain.Enums;

namespace EasySave.Domain.Exceptions
{
    public class LogServerUnavailableException : EasySaveException
    {
        public LogServerUnavailableException(string host, int port, Exception inner)
            : base(EasySaveErrorCode.LogServerUnavailable)
        { }
    }
}