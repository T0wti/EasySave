using EasySave.Domain.Interfaces;
using System.Management;

namespace EasySave.Domain.Services
{
    // Test with observer 
    // Polls the business software process at a fixed interval
    // Pauses all active backup jobs when detected, resumes them when it stops

    public class BusinessSoftwareWatcher : IBusinessSoftwareWatcher
    {
        private readonly IBusinessSoftwareService _businessSoftwareService;
        private readonly IBackupHandleRegistry _registry;
        private readonly TimeSpan _pollInterval;

        public BusinessSoftwareWatcher(
            IBusinessSoftwareService businessSoftwareService,
            IBackupHandleRegistry registry,
            TimeSpan? pollInterval = null)
        {
            _businessSoftwareService = businessSoftwareService;
            _registry = registry;
            _pollInterval = pollInterval ?? TimeSpan.FromMilliseconds(500);
        }

        public async Task WatchAsync(CancellationToken stopWhen)
        {
            var processName = _businessSoftwareService.GetConfiguredName();

            if (string.IsNullOrWhiteSpace(processName))
            {
                await Task.Delay(Timeout.Infinite, stopWhen);
                return;
            }

            var exe = $"{processName}.exe";

            using var startWatcher = CreateWatcher(
                $@"SELECT * FROM __InstanceCreationEvent WITHIN 1
           WHERE TargetInstance ISA 'Win32_Process'
           AND TargetInstance.Name = '{exe}'",
                PauseAll);

            using var stopWatcher = CreateWatcher(
                $@"SELECT * FROM __InstanceDeletionEvent WITHIN 1
           WHERE TargetInstance ISA 'Win32_Process'
           AND TargetInstance.Name = '{exe}'",
                ResumeAll);

            if (_businessSoftwareService.IsBusinessSoftwareRunning())
                PauseAll();

            startWatcher.Start();
            stopWatcher.Start();

            try
            {
                await Task.Delay(Timeout.Infinite, stopWhen);
            }
            catch (TaskCanceledException) { }
            finally
            {
                startWatcher.Stop();
                stopWatcher.Stop();
                ResumeAll();
            }
        }

        private static ManagementEventWatcher CreateWatcher(string query, Action callback)
        {
            var watcher = new ManagementEventWatcher(new WqlEventQuery(query));
            watcher.EventArrived += (_, _) => callback();
            return watcher;
        }

        private void PauseAll()
        {
            foreach (var (_, handle) in _registry.GetAll())
                handle.Pause();
        }

        private void ResumeAll()
        {
            foreach (var (_, handle) in _registry.GetAll())
                handle.Resume();
        }
    }
}