using EasySave.Domain.Models;

namespace EasySave.Domain.Interfaces
{
    public interface ILogService
    {
        void Write(LogEntry entry);
    }

}