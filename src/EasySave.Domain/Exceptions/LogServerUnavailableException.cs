using EasySave.Domain.Enums;

// Thrown when the application cannot reach the remote log server over TCP
namespace EasySave.Domain.Exceptions
{
    public class LogServerUnavailableException : EasySaveException
    {
        public LogServerUnavailableException(string host, int port, Exception inner)
            : base(EasySaveErrorCode.LogServerUnavailable)
        { }
    }
}