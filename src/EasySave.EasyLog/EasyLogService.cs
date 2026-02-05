using EasySave.EasyLog.Interfaces;
using EasySave.EasyLog.Writers;

namespace EasySave.EasyLog
{
    public class EasyLogService : ILogService
    {
        private readonly JsonLogWriter _writer;

        public EasyLogService(string logDirectoryPath)
        {
            _writer = new JsonLogWriter(logDirectoryPath);
        }

        public void WriteJson(object entry)
        {
            _writer.WriteJson(entry);
        }
    }
}