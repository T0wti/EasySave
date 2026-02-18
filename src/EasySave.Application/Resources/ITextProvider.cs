namespace EasySave.Application.Resources
{

    public interface ITextProvider
    {
        // General content
        string Header { get; }
        string Footer { get; }
        string MainMenuTitle { get; }
        string AskEntryFromUser { get; }
        string ExitOption { get; }
        string WrongInput { get; }
        string Exit { get; }
        string Confirm { get; }
        string Home { get; }
        string Save { get; }

        // Home Page
        string HomeWelcome { get; }
        string HomeConfiguredBackup { get; }

        // Options in the base menu | Argan
        string DeleteBackup { get; }
        string DeleteBackupTitle { get; }
        string CreateBackup { get; }
        string CreateBackupTitle { get; }
        string EditBackup { get; }
        string EditBackupTitle { get; }
        string LanguageOption { get; }
        string ListBackup { get; }
        string ListBackupTitle { get; }
        string ExeBackup { get; }
        string ExeBackupTitle { get; }
        string LogFormat { get; }
        string SettingsMenu { get; }

        // Options for the creation 
        string CreateBackupMenuTitle { get; }
        string EnterBackupName { get; }
        string EnterSourcePath { get; }
        string EnterTargetPath { get; }
        string EnterBackupType { get; }
        string WaterMarkBackupName { get; }
        string WaterMarkBackupSourcePath { get; }
        string WaterMarkBackupTargetPath { get; }
        string Full { get; }
        string Differential { get; }
        string ExtensionToEncryptTitle { get; }
        string Encrypt { get; }

        //

        string BackupCreated { get; }
        string BackupEdited { get; }
        string EnterBackupToDelete { get; }
        string BackupDeleted { get; }
        string AskIdToEdit { get; }
        string BrowseFile { get; }
            // errors
            string NameEmpty { get; }
            string NameTooLong { get; }
            string SourcePathEmpty { get; }
            string SourcePathNotAbsolute { get; }
            string SourcePathNotFound { get; }
            string TargetPathEmpty { get; }
            string TargetPathNotAbsolute { get; }
            string TargetPathNotFound { get; }
            string SourceEqualsTarget { get; }

        // Options in the change language menu | Le
        string LanguageMenuTitle { get; }
        string Language1 { get; }
        string Language2 { get; }

        // Options in the first start menu | Loup
        string ChooseFirstLanguageMenuTitle { get; }
        string ChooseFirstLanguage { get; }

        // List Backup Menu
        string ListBackupMenuTitle { get; }

        // Backup Detail Menu
        string BackupNameMenuTitle { get; }
        string BackupName { get; }
        string BackupNameTitle { get; }
        string BackupSourcePath { get; }
        string BackupSourcePathTitle {  get; }
        string BackupTargetPath { get; }
        string BackupTargetPathTitle { get; }
        string BackupType { get; }
        string BackupTypeTitle { get; }

        // Execute Backup Menu
        string ExeBackupMenuTitle { get; }
        string BackupNames { get; }
        string ExeSelected { get; }
        string ExeBackupInstruction { get; }

        // Execute Backup Menu Details
        string ExeBackupMenuDetailTitle { get; }
        string ExeBackupInProgress { get; }
        string ExeBackupCompleted { get; }

        // Change Log Format Menu
        string LogFormatMenuTitle { get; }
        string LogFormat1 { get; }
        string LogFormat2 { get; }
        string LogFormatChanged { get; }
        string CurrentLogFormat { get; }

        // Settings menu
        string SettingsMenuTitle { get; }
        string SettingsMenuBusiness { get; }
        string SettingsMenuLanguage { get; }
        string SettingsMenuLogFormat { get; }
        string SettingsMenuExemple { get; }

        // Message Box
        string MessageBoxInfoTitle { get; }
        string MessageBoxJobCreated { get; }
        string MessageBoxJobExecuted { get; }
        string MessageBoxJobDeleted { get; }
        string MessageBoxJobEdited { get; }
        string MessageBoxOk { get; }
    }
}