using EasySave.Domain.Interfaces;
using EasySave.Domain.Services;
using EasySave.EasyLog;

namespace EasySave.Application.Controllers
{
    public static class ControllerFactory
    {
        public static BackupController CreateBackupController()
        {
            // Utilisation du Singleton pour la configuration
            IConfigurationService configService = ConfigurationService.Instance;
            var settings = configService.LoadSettings();

            // INITIALISATION DES SINGLETONS
            FileStateService.Instance.Initialize(settings.StateFileDirectoryPath);
            EasyLogService.Instance.Initialize(settings.LogDirectoryPath);
            IFileBackupService fileBackupService = FileBackupService.Instance;


            // Instanciation des services classiques et Singletons
            IFileService fileService = new FileService();
            IBackupStrategy fullStrategy = new FullBackupStrategy(fileService);
            IBackupStrategy diffStrategy = new DifferentialBackupStrategy(fileService);

            // Correction : Utilisation du Singleton pour FileBackupService

            // 4. Création de l'exécuteur (BackupService)
            IBackupService executor = new BackupService(fileService, fullStrategy, diffStrategy, fileBackupService);

            // 5. Injection dans le BackupManagerService 
            IBackupManagerService manager = new BackupManagerService(fileBackupService, executor, settings);

            return new BackupController(manager, executor);
        }

        public static ConfigurationController CreateConfigurationController()
        {
            // Utilisation du Singleton ici aussi
            IConfigurationService configService = ConfigurationService.Instance;
            return new ConfigurationController(configService);
        }
    }
}