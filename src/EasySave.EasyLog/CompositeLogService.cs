using EasySave.EasyLog.Interfaces;

namespace EasySave.EasyLog
{
    //Orchestrates local EasyLogService and an optional remote action
    public class CompositeLogService : ILogService
    {
        private readonly EasyLogService _local;
        private readonly Func<object, Task>? _remote;
        private readonly int _logMode;

        public CompositeLogService(
            EasyLogService local,
            Func<object, Task>? remote,
            int logMode)
        {
            _local = local;
            _remote = remote;
            _logMode = logMode;
        }

        public void Write<T>(T entry)
        {
            switch (_logMode)
            {
                case 0: // Local only
                    _local.Write(entry);
                    break;

                case 1: // Centralized only
                    if (_remote is not null)
                        _ = _remote(entry!);
                    break;

                case 2: // Both
                    _local.Write(entry);
                    if (_remote is not null)
                        _ = _remote(entry!);
                    break;
            }
        }
    }
}