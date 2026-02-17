namespace EasySave.Application.Resources
{

    public class EnglishTextProvider : GeneralTextProvider
    {
        // Base menu
        public override string MainMenuTitle => "Main Menu";
        public override string CreateBackup => "1. Create a file backup";
        public override string LanguageOption => "9. Change console language";
        public override string ExitOption => "0. Exit";
        public override string AskEntryFromUser => "Enter the desired number or command: ";
        public override string EditBackup => "3. Edit a file backup";
        public override string DeleteBackup => "2. Delete a file backup";
        public override string WrongInput => "Invalid input, please try again: ";
        public override string Exit => "Exit";
        public override string Home => "Home";
        public override string Confirm => "Confirm";
        public override string Save => "Save";
        public override string ListBackup => "4. List all file backups";
        public override string ExeBackup => "5. Execute a file backup";
        public override string LogFormat => "8. Change log format";
        public override string SettingsMenu => "Settings";

        // Home Page
        public override string HomeWelcome => "Welcome";
        public override string HomeConfiguredBackup => "Configured backup";

        // Creation BackupMenu 
        public override string CreateBackupMenuTitle => "Create a new backup";
        public override string EnterBackupName => "Please enter the name of your backup job: ";
        public override string EnterSourcePath => "Please enter the source path of the folder to back up (C:\\\\Users\\\\Downloads): ";
        public override string EnterTargetPath => "Please enter the destination path for the backup ((C:\\\\Users\\\\Save): ";
        public override string EnterBackupType => "Please enter the backup type (1 - Full | 2 - Differential): ";
        public override string BrowseFile => "Browse";
        public override string WaterMarkBackupName => "Backup name";
        public override string WaterMarkBackupSourcePath => "Source folder";
        public override string WaterMarkBackupTargetPath => "Destination folder";
        public override string Full => "Full";
        public override string Differential => "Differential";
        public override string ExtensionToEncryptTitle => "Extension to encrypt";
        public override string Encrypt => "Encrypt";
            // errors
            public override string NameEmpty => "Backup name can't be empty";
            public override string NameTooLong => "Backup name is too long";
            public override string SourcePathEmpty => "Source folder can't be empty";
            public override string SourcePathNotAbsolute  => "Source folder must be absolute";
            public override string SourcePathNotFound => "Source folder not found";
            public override string TargetPathEmpty => "Target folder can't be empty";
            public override string TargetPathNotAbsolute => "Target folder must be absolute";
            public override string TargetPathNotFound => "Target folder not found";
            public override string SourceEqualsTarget => "Source and target can't be the same";

        //
        public override string BackupCreated => "Backup job created successfully.";
        public override string BackupEdited => "Backup job edited successfully.";
        public override string EnterBackupToDelete => "Enter the ID of the backup job to delete: ";
        public override string BackupDeleted => "Backup job deleted successfully.";


        public override string AskIdToEdit => "Enter the ID of the backup job to edit:";

        // Language menu
        public override string LanguageMenuTitle => "Change the language";
        public override string Language1 => "1. French";
        public override string Language2 => "2. English";

        // Choose first language menu
        public override string ChooseFirstLanguageMenuTitle => "Choose the default language";
        public override string ChooseFirstLanguage => "Choose the default language :";

        // List Backup Menu
        public override string ListBackupMenuTitle => "All configured backups";

        // Backup Detail Menu
        public override string BackupNameMenuTitle => "Files backup details";
        public override string BackupName => "id-a-" + ". " + "nom-job"; // mettre la fonction pour sortir le nom d'une backup
        public override string BackupSourcePath => "";
        public override string BackupTargetPath => "";
        public override string BackupType => "";

        // Execute Backup Menu
        public override string ExeBackupMenuTitle => "Choose the file backup to execute";
        //public override string BackupNames => Format("nom save"); // not used for the moment
        public override string ExeBackupInstruction => "Command exemple : id_Backup";

        // Execute Backup Menu Details
        public override string ExeBackupMenuDetailTitle => "Backup Status";
        public override string ExeBackupInProgress => "Backup in progress...";
        public override string ExeBackupCompleted => "Backup complete.";

        // Change Log Format Menu
        public override string LogFormatMenuTitle => "Change Log Format";
        public override string LogFormat1 => "1. JSON";
        public override string LogFormat2 => "2. XML";
        public override string LogFormatChanged => "Log format changed successfully!";
        public override string CurrentLogFormat => "Current format: ";

        // Settings menu
        public override string SettingsMenuTitle => "Easy Save's settings";
        public override string SettingsMenuBusiness => "Software suspending execution";
        public override string SettingsMenuLanguage => "Language";
        public override string SettingsMenuLogFormat => "Log files format";
        public override string SettingsMenuExemple => "Example: Calculator.exe";

        // Message Box
        public override string MessageBoxInfoTitle => "Information";
        public override string MessageBoxJobDeleted => "Job deleted successfully";
        public override string MessageBoxJobEdited => "Job edited successfully";
        public override string MessageBoxOk => "Ok";
    }
}