using EasySave.Domain.Interfaces;
using System.Management;

namespace EasySave.Domain.Services
{
    // Watches a configured business software process
    // When the process starts all backup jobs are paused

    public class BusinessSoftwareWatcher : IBusinessSoftwareWatcher
    {
        private readonly IBusinessSoftwareService _businessSoftwareService;
        private readonly IBackupHandleRegistry _registry;

        public BusinessSoftwareWatcher(
            IBusinessSoftwareService businessSoftwareService,
            IBackupHandleRegistry registry)
        {
            _businessSoftwareService = businessSoftwareService;
            _registry = registry;
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

            // Watcher that triggers when the process starts
            using var startWatcher = CreateWatcher(
                $@"SELECT * FROM __InstanceCreationEvent WITHIN 1
           WHERE TargetInstance ISA 'Win32_Process'
           AND TargetInstance.Name = '{exe}'",
                PauseAll);

            // Watcher that triggers when the process stops
            using var stopWatcher = CreateWatcher(
                $@"SELECT * FROM __InstanceDeletionEvent WITHIN 1
           WHERE TargetInstance ISA 'Win32_Process'
           AND TargetInstance.Name = '{exe}'",
                ResumeAll);

            // If the business software is already running at startup : pause all
            if (_businessSoftwareService.IsBusinessSoftwareRunning())
                PauseAll();

            // Start listening to process start/stop events
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
            var watcher = new ManagementEventWatcher(new WqlEventQuery(query)); // Create a WMI watcher using the provided query
            watcher.EventArrived += (_, _) => callback(); // Attach the callback to be executed when the event is detected

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