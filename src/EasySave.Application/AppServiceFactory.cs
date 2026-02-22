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
            // ConfigurationService 
            IConfigurationService configService = new ConfigurationService();
            var settings = configService.LoadSettings();

            // FileStateService
            IFileStateService fileStateService = new FileStateService(settings.StateFileDirectoryPath);

            // EasyLogService (singleton)
            EasyLogService.Instance.Initialize(settings.LogDirectoryPath, settings.LogFormat);
            ILogService logService = EasyLogService.Instance;

            // FileBackupService
            string jobsDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "EasySave"
            );

            IFileBackupService fileBackupService = new FileBackupService();

            // Businesses Services 
            IFileService fileService = new FileService();
            IBackupStrategy fullStrategy = new FullBackupStrategy(fileService);
            IBackupStrategy diffStrategy = new DifferentialBackupStrategy(fileService);
            IStateService stateService = new StateService(fileStateService);
            IBackupHandleRegistry registry = new BackupHandleRegistry();
            IBusinessSoftwareService businessSoftwareService = new BusinessSoftwareService(settings);
            IBusinessSoftwareWatcher watcher = new BusinessSoftwareWatcher(businessSoftwareService, registry);
            ICryptoSoftService cryptoSoftService = new CryptoSoftService(
                settings.CryptoSoftPath,
                settings.CryptoSoftKeyPath,
                settings.EncryptedFileExtensions ?? new List<string>());
            IPriorityGate priorityGate = new PriorityGate(settings.PriorityFileExtensions ?? new List<string>());


            IBackupService executor = new BackupService(
                fileService,
                fullStrategy,
                diffStrategy,
                stateService,
                logService,
                watcher,
                cryptoSoftService,
                priorityGate);

            IBackupManagerService manager = new BackupManagerService(
                fileBackupService,
                executor,
                settings
            );

            return new BackupAppService(manager, executor, fileStateService, registry);
        }

        public static ConfigAppService CreateConfigurationController()
        {
            IConfigurationService configService = new ConfigurationService();
            return new ConfigAppService(configService);
        }
    }
}