using System;
using EasySave.EasyLog.Interfaces;
using EasySave.EasyLog.Writers;

namespace EasySave.EasyLog
{
    // Singleton service for logging entries in JSON or XML format.
    // Format codes: 0 = JSON, 1 = XML
    public class EasyLogService : ILogService
    {
        private static readonly Lazy<EasyLogService> _instance = new(() => new EasyLogService());
        public static EasyLogService Instance => _instance.Value;

        private ILogWriter? _writer;
        private bool _isInitialized;

        private EasyLogService() { }

        public void Initialize(string logDirectoryPath, int formatCode)
        {
            if (_isInitialized)
                return; 

            if (string.IsNullOrWhiteSpace(logDirectoryPath))
                throw new ArgumentException("Log directory path cannot be null or empty.", nameof(logDirectoryPath));

            _writer = formatCode switch
            {
                0 => new JsonLogWriter(logDirectoryPath),
                1 => new XmlLogWriter(logDirectoryPath),
                _ => throw new ArgumentException($"Unsupported log format code: {formatCode}")
            };

            _isInitialized = true;
        }

        // Writes a log entry using the configured writer.
        public void Write<T>(T entry)
        {
            if (!_isInitialized || _writer == null)
                throw new InvalidOperationException("EasyLogService must be initialized via Initialize() before use.");

            _writer.Write(entry);
        }

        // Resets the service state. Useful for changing configuration. (So, to run other format the controller must reset the configuration and initialize another with the good logformat)
        public void Reset()
        {
            _writer = null;
            _isInitialized = false;
        }
    }
}