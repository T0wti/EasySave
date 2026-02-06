using EasySave.EasyLog.Interfaces;
using EasySave.EasyLog.Writers;

namespace EasySave.EasyLog
{
    public class EasyLogService : ILogService
    {
        private static readonly Lazy<EasyLogService> _instance = new(() => new EasyLogService());
        public static EasyLogService Instance => _instance.Value;
        
        private JsonLogWriter? _writer;
        
        private EasyLogService() { }

        public void Initialize(string logDirectoryPath)
        {
            if (_writer != null) return;
            _writer = new JsonLogWriter(logDirectoryPath);
        }

        public void WriteJson(object entry)
        {
            if (_writer == null)
                throw new InvalidOperationException("EasyLogService must be initialized via Initialize() before use.");
            _writer.WriteJson(entry);
        }
    }
}