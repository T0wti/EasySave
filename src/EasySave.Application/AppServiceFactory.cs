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
            IBusinessSoftwareService businessSoftwareService = new BusinessSoftwareService(settings);
            ICryptoSoftService cryptoSoftService = new CryptoSoftService(settings);


            IBackupService executor = new BackupService(
                fileService,
                fullStrategy,
                diffStrategy,
                stateService,
                logService,
                businessSoftwareService,
                cryptoSoftService);

            IBackupManagerService manager = new BackupManagerService(
                fileBackupService,
                executor,
                settings
            );

            return new BackupAppService(manager, executor);
        }

        public static ConfigAppService CreateConfigurationController()
        {
            IConfigurationService configService = new ConfigurationService();
            return new ConfigAppService(configService);
        }
    }
}