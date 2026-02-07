using EasySave.Domain.Interfaces;
using EasySave.Domain.Services;
using EasySave.EasyLog;

namespace EasySave.Application.Controllers
{
    public static class ControllerFactory
    {
        public static BackupController CreateBackupController()
        {
            // 1. Charger la configuration (qui va créer le fichier par défaut si besoin)
            IConfigurationService configService = new ConfigurationService();
            var settings = configService.LoadSettings();

            // 2. INITIALISATION DES SINGLETONS
            FileStateService.Instance.Initialize(settings.StateFileDirectoryPath);
            EasyLogService.Instance.Initialize(settings.LogDirectoryPath); 

            // 3. Instanciation des services classiques
            IFileService fileService = new FileService();
            IBackupStrategy fullStrategy = new FullBackupStrategy(fileService);
            IBackupStrategy diffStrategy = new DifferentialBackupStrategy(fileService);
            IFileBackupService fileBackupService = new FileBackupService();

            IBackupManagerService manager = new BackupManagerService(fileBackupService, settings);

            // 4. Injection dans le BackupService
            IBackupService executor = new BackupService(fileService, fullStrategy, diffStrategy, fileBackupService);

            return new BackupController(manager, executor);
        }

        public static ConfigurationController CreateConfigurationController()
        {
            IConfigurationService configService = new ConfigurationService();
            return new ConfigurationController(configService);
        }
    }
}
