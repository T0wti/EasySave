using EasySave.Domain.Interfaces;
using EasySave.Domain.Models;
using System.Diagnostics;

namespace EasySave.Domain.Services
{
    // Detects if the configured business software is currently running
    public class BusinessSoftwareService : IBusinessSoftwareService
    {
        private readonly IConfigurationService _configService;

        public BusinessSoftwareService(IConfigurationService configService)
        {
            _configService = configService;
        }

        // Checks the OS process list for the configured business software name
        public bool IsBusinessSoftwareRunning()
        {
            var name = _configService.LoadSettings().BusinessSoftwareName;
            if (string.IsNullOrWhiteSpace(name)) return false;
            return Process.GetProcessesByName(name).Length > 0;
        }

        // Returns the raw configured name, or an empty string if none is set
        public string GetConfiguredName()
        {
            return _configService.LoadSettings().BusinessSoftwareName ?? string.Empty;
        }
    }
}