using EasySave.Domain.Interfaces;
using EasySave.Domain.Services;
using EasySave.EasyLog;
using EasySave.EasyLog.Interfaces;

namespace EasySave.Application.Controllers
{
    public static class ControllerFactory
    {
        public static BackupController CreateBackupController()
        {
            // 1. Load configuration via singleton
            IConfigurationService configService = ConfigurationService.Instance;
            var settings = configService.LoadSettings();

            // 2. Initialize global singletons
            FileStateService.Instance.Initialize(settings.StateFileDirectoryPath);
            EasyLogService.Instance.Initialize(settings.LogDirectoryPath);

            // 3. Singletons for persistent services
            IFileBackupService fileBackupService = FileBackupService.Instance;
            ILogService logService = EasyLogService.Instance;
            IStateService stateService = new StateService(FileStateService.Instance); // Could also be singleton if desired

            // 4. Instantiate regular services
            IFileService fileService = new FileService();
            IBackupStrategy fullStrategy = new FullBackupStrategy(fileService);
            IBackupStrategy diffStrategy = new DifferentialBackupStrategy(fileService);

            // 5. Create BackupService with **all dependencies injected**
            IBackupService executor = new BackupService(
                fileService,
                fullStrategy,
                diffStrategy,
                fileBackupService,
                stateService,   
                logService     
            );

            // 6. Create BackupManagerService with executor injected
            IBackupManagerService manager = new BackupManagerService(
                fileBackupService,
                executor,
                settings
            );

            // 7. Return controller with fully wired dependencies
            return new BackupController(manager, executor);
        }

        public static ConfigurationController CreateConfigurationController()
        {
            IConfigurationService configService = ConfigurationService.Instance;
            return new ConfigurationController(configService);
        }
    } 
}
