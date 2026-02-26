using EasySave.EasyLog.Interfaces;

namespace EasySave.EasyLog
{
    // Singleton that routes log entries to local EasyLog, remote TCP server, or both
    // depending on the configured LogMode.
    // Same pattern as EasyLogService: call Reset() then Initialize() to reconfigure at runtime.
    // LogMode: 0 = Local only, 1 = Centralized only, 2 = Both
    public class LogDispatcher : ILogService
    {
        private static readonly Lazy<LogDispatcher> _instance = new(() => new LogDispatcher());
        public static LogDispatcher Instance => _instance.Value;
        private readonly object _initLock = new();

        private EasyLogService? _local;
        private Func<object, Task>? _remote;
        private int _logMode;
        private bool _isInitialized;

        private LogDispatcher() { }

        public void Initialize(EasyLogService local, Func<object, Task>? remote, int logMode)
        {
            lock (_initLock)
            {
                if (_isInitialized) return;

                _local = local;
                _remote = remote;
                _logMode = logMode;
                _isInitialized = true;
            }
        }

        public void Reset()
        {
            lock (_initLock)
            {
                _local = null;
                _remote = null;
                _isInitialized = false;
            }
        }

        public async Task Write<T>(T entry)
        {
            if (!_isInitialized || _local == null)
                throw new InvalidOperationException("CompositeLogService must be initialized via Initialize() before use.");

            switch (_logMode)
            {
                case 0: // Local only
                    await _local.Write(entry).ConfigureAwait(false);
                    break;

                case 1: // Centralized only
                    if (_remote is not null)
                        await _remote(entry!).ConfigureAwait(false);
                    break;

                case 2: // Both
                    await _local.Write(entry).ConfigureAwait(false);
                    if (_remote is not null)
                        await _remote(entry!).ConfigureAwait(false);
                    break;

                default:
                    await _local.Write(entry).ConfigureAwait(false);
                    break;
            }
        }
    }
}