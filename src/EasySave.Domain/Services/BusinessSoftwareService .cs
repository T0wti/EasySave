using EasySave.Domain.Interfaces;
using EasySave.Domain.Models;
using System.Diagnostics;

namespace EasySave.Domain.Services
{
    // Detects if the configured business software is currently running
    public class BusinessSoftwareService : IBusinessSoftwareService
    {
        private readonly ApplicationSettings _settings;

        public BusinessSoftwareService(ApplicationSettings settings)
        {
            _settings = settings;
        }

        // Checks the OS process list for the configured business software name
        public bool IsBusinessSoftwareRunning()
        {
            if (string.IsNullOrWhiteSpace(_settings.BusinessSoftwareName))
                return false;

            var processes = Process.GetProcessesByName(_settings.BusinessSoftwareName);
            return processes.Length > 0;
        }

        // Returns the raw configured name, or an empty string if none is set
        public string GetConfiguredName()
        {
            return _settings.BusinessSoftwareName ?? string.Empty;
        }
    }
}