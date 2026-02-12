using EasySave.Domain.Interfaces;
using EasySave.Domain.Models;
using EasySave.Domain.Services;
using EasySave.EasyLog;
using EasySave.EasyLog.Interfaces;
using System;
using System.IO;

namespace EasySave.Application.Controllers
{
    public static class ControllerFactory
    {
        public static BackupController CreateBackupController()
        {
            // 1. ConfigurationService (V1-style constructor)
            IConfigurationService configService = new ConfigurationService();
            var settings = configService.LoadSettings();

            // 2. FileStateService (V2.1 constructor paramétré)
            IFileStateService fileStateService = new FileStateService(settings.StateFileDirectoryPath);

            // 3. EasyLogService (singleton légitime)
            EasyLogService.Instance.Initialize(settings.LogDirectoryPath, settings.LogFormat);
            ILogService logService = EasyLogService.Instance;

            // 4. FileBackupService (V2.1 constructor paramétré)
            // On stocke les jobs dans %APPDATA%/EasySave/jobs.json
            string jobsDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "EasySave"
            );

            IFileBackupService fileBackupService = new FileBackupService();

            // 5. Services métiers
            IFileService fileService = new FileService();
            IBackupStrategy fullStrategy = new FullBackupStrategy(fileService);
            IBackupStrategy diffStrategy = new DifferentialBackupStrategy(fileService);

            IStateService stateService = new StateService(fileStateService);

            IBackupService executor = new BackupService(
                fileService,
                fullStrategy,
                diffStrategy,
                stateService,
                logService
            );

            IBackupManagerService manager = new BackupManagerService(
                fileBackupService,
                executor,
                settings
            );

            return new BackupController(manager, executor);
        }

        public static ConfigurationController CreateConfigurationController()
        {
            IConfigurationService configService = new ConfigurationService();
            return new ConfigurationController(configService);
        }
    }
}