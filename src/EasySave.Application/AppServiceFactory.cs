using EasySave.Domain.Interfaces;
using EasySave.Domain.Services;
using EasySave.EasyLog;
using EasySave.EasyLog.Interfaces;

namespace EasySave.Application
{
    public static class AppServiceFactory
    {
        public static BackupAppService CreateBackupController()
        {
            IConfigurationService configService = new ConfigurationService();
            var settings = configService.LoadSettings();

            IFileStateService fileStateService = new FileStateService(settings.StateFileDirectoryPath);

            // Local writer
            EasyLogService.Instance.Reset();
            EasyLogService.Instance.Initialize(settings.LogDirectoryPath, settings.LogFormat);

            // Optional remote TCP client
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

            ILogService logService = new CompositeLogService(
               EasyLogService.Instance, remote, settings.LogMode);

            IFileBackupService fileBackupService = new FileBackupService();
            IFileService fileService = new FileService();
            IBackupStrategy fullStrategy = new FullBackupStrategy(fileService);
            IBackupStrategy diffStrategy = new DifferentialBackupStrategy(fileService);
            IStateService stateService = new StateService(fileStateService);
            IBackupHandleRegistry registry = new BackupHandleRegistry();
            IBusinessSoftwareService businessSoftwareService = new BusinessSoftwareService(configService);
            IBusinessSoftwareWatcher watcher = new BusinessSoftwareWatcher(businessSoftwareService, registry);
            ICryptoSoftService cryptoSoftService = new CryptoSoftService(configService);
            IPriorityGate priorityGate = new PriorityGate(configService);
            ILargeSizeGate largeSizeGate = new LargeSizeGate(configService);

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