using EasySave.Domain.Interfaces;
using EasySave.Domain.Services;
using EasySave.EasyLog;
using EasySave.EasyLog.Interfaces;

namespace EasySave.Application
{
    // Static factory responsible for building and wiring all application services.
    public static class AppServiceFactory
    {
        // Loads user settings, initialises logging (local and optionally remote via TCP),
        // builds all backup-related services, and wires them together
        public static BackupAppService CreateBackupController()
        {
            // Load application settings (paths, log format, server info, etc...)
            IConfigurationService configService = new ConfigurationService();
            var settings = configService.LoadSettings();

            // Service responsible for persisting and reading the state of each backup job
            IFileStateService fileStateService = new FileStateService(settings.StateFileDirectoryPath);

            // Initialise the local log writer (JSON or XML depending on settings)
            EasyLogService.Instance.Reset();
            EasyLogService.Instance.Initialize(settings.LogDirectoryPath, settings.LogFormat);

            // Optionally set up a remote TCP log sink when LogMode requires it
            Func<object, Task>? remote = null;
            if (settings.LogMode is 1 or 2 && !string.IsNullOrWhiteSpace(settings.LogServerHost))
            {
                var logClient = new TcpLogClient(
                    settings.LogServerHost,
                    settings.LogServerPort,
                    isXml: settings.LogFormat == 1,
                    fallbackDirectory: settings.LogDirectoryPath);

                remote = entry => logClient.SendAsync(entry);
            }

            // LogDispatcher fans out log entries to the local writer and/or the remote sink
            LogDispatcher.Instance.Reset();
            LogDispatcher.Instance.Initialize(EasyLogService.Instance, remote, settings.LogMode);
            ILogService logService = LogDispatcher.Instance;

            // Low-level file copy service and high-level backup file management
            IFileBackupService fileBackupService = new FileBackupService();
            IFileService fileService = new FileService();

            // Backup strategies: full copies every file, differential copies only changed files
            IBackupStrategy fullStrategy = new FullBackupStrategy(fileService);
            IBackupStrategy diffStrategy = new DifferentialBackupStrategy(fileService);

            // Wraps file state persistence behind a domain-level abstraction
            IStateService stateService = new StateService(fileStateService);

            // Registry that tracks active backup handles, used to coordinate pause/resume/stop
            IBackupHandleRegistry registry = new BackupHandleRegistry();

            // Detects whether a blacklisted business software (e.g. ERP) is running
            IBusinessSoftwareService businessSoftwareService = new BusinessSoftwareService(configService);
            IBusinessSoftwareWatcher watcher = new BusinessSoftwareWatcher(businessSoftwareService, registry);

            // Encrypts files at the end of a backup job using an external CryptoSoft process
            ICryptoSoftService cryptoSoftService = new CryptoSoftService(configService);

            // Gates that control execution order and concurrency:
            // PriorityGate ensures priority files are transferred before others
            // LargeSizeGate limits the number of large files being copied simultaneously
            IPriorityGate priorityGate = new PriorityGate(configService);
            ILargeSizeGate largeSizeGate = new LargeSizeGate(configService);

            // Core backup executor: orchestrates file copies, state updates, logging, encryption, and all concurrency/throttling concerns
            IBackupService executor = new BackupService(
                fileService, 
                fullStrategy, 
                diffStrategy, 
                stateService,
                logService, 
                watcher, 
                cryptoSoftService, 
                priorityGate, 
                largeSizeGate);

            // High-level manager: handles CRUD on backup job definitions
            IBackupManagerService manager = new BackupManagerService(fileBackupService, executor, settings);

            return new BackupAppService(manager, executor, fileStateService, registry);
        }

        public static ConfigAppService CreateConfigurationController()
        {
            IConfigurationService configService = new ConfigurationService();
            return new ConfigAppService(configService);
        }
    }
}