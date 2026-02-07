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
            // On passe les chemins récupérés depuis les settings
            FileStateService.Instance.Initialize(settings.StateFileDirectoryPath);
            EasyLogService.Instance.Initialize(settings.LogDirectoryPath); // <-- C'est l'étape manquante !

            // 3. Instanciation des services classiques
            IFileService fileService = new FileService();
            IBackupStrategy fullStrategy = new FullBackupStrategy(fileService);
            IBackupStrategy diffStrategy = new DifferentialBackupStrategy(fileService);
            IFileBackupService fileBackupService = new FileBackupService();

            IBackupManagerService manager = new BackupManagerService(fileBackupService);

            // 4. Injection dans le BackupService
            // Assure-toi que BackupService utilise bien les instances initialisées
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
