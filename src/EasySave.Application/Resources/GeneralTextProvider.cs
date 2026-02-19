namespace EasySave.Application.Resources
{

    public class GeneralTextProvider : ITextProvider
    {
        // Define an automatic indentation (to improve readability)
        private const string Indent = "       ";
        private const string BigTitle = "-----";
        private const string Title = "--";
        protected string Format(string text) => $"{Indent}{text}";
        protected string FormatBigTitle(string text) => $"{Indent}{BigTitle}{" "}{text}{" "}{BigTitle}";
        protected string FormatTitle(string text) => $"{Indent}{Title}{" "}{text}{" "}{Title}";

        private readonly string _defaultString;

        // Constructor
        protected GeneralTextProvider()
        {
            _defaultString = "";
        }

        // Header and Footer
        public string Header => "================ Easy Save Console ================";
        public string Footer => "===================================================";
        // Blank string which will be overridden in the language wanted
        // Base menu
        public virtual string MainMenuTitle => _defaultString;
        public virtual string CreateBackup => _defaultString;
        public virtual string CreateBackupTitle => _defaultString;
        public virtual string LanguageOption => _defaultString;
        public virtual string ExitOption => _defaultString;
        public virtual string AskEntryFromUser => _defaultString;
        public virtual string DeleteBackup => _defaultString;
        public virtual string DeleteBackupTitle => _defaultString;
        public virtual string EditBackup => _defaultString;
        public virtual string EditBackupTitle => _defaultString;
        public virtual string WrongInput => _defaultString;
        public virtual string Exit => _defaultString;
        public virtual string Home => _defaultString;
        public virtual string Confirm => _defaultString;
        public virtual string Save => _defaultString;
        public virtual string ListBackup => _defaultString;
        public virtual string ListBackupTitle => _defaultString;
        public virtual string ExeBackup => _defaultString;
        public virtual string ExeBackupTitle => _defaultString;
        public virtual string LogFormat => _defaultString;
        public virtual string SettingsMenu => _defaultString;

        //Home Page
        public virtual string HomeWelcome => _defaultString;
        public virtual string HomeConfiguredBackup => _defaultString;

        //Creation menu

        public virtual string CreateBackupMenuTitle => _defaultString;
        public virtual string EnterBackupName => _defaultString;
        public virtual string EnterSourcePath => _defaultString;
        public virtual string EnterTargetPath => _defaultString;
        public virtual string EnterBackupType => _defaultString;
        public virtual string BrowseFile => _defaultString;
        public virtual string WaterMarkBackupName => _defaultString;
        public virtual string WaterMarkBackupSourcePath => _defaultString;
        public virtual string WaterMarkBackupTargetPath => _defaultString;
        public virtual string Full => _defaultString;
        public virtual string Differential => _defaultString;
        public virtual string ExtensionToEncryptTitle => _defaultString;
        public virtual string Encrypt => _defaultString;

        //
        public virtual string BackupCreated => _defaultString;
        public virtual string BackupEdited => _defaultString;
        public virtual string EnterBackupToDelete => _defaultString;
        public virtual string BackupDeleted => _defaultString;
        public virtual string AskIdToEdit => _defaultString;
            // errors
            public virtual string NameEmpty => _defaultString;
            public virtual string NameTooLong => _defaultString;
            public virtual string SourcePathEmpty => _defaultString;
            public virtual string SourcePathNotAbsolute => _defaultString;
            public virtual string SourcePathNotFound => _defaultString;
            public virtual string TargetPathEmpty => _defaultString;
            public virtual string TargetPathNotAbsolute => _defaultString;
            public virtual string TargetPathNotFound => _defaultString;
            public virtual string SourceEqualsTarget => _defaultString;
            public virtual string BusinessSoftwareRunning => _defaultString;

        // Change Language Menu
        public virtual string LanguageMenuTitle => _defaultString;
        public virtual string Language1 => _defaultString;
        public virtual string Language2 => _defaultString;

        // Choose first language menu
        public virtual string ChooseFirstLanguageMenuTitle => _defaultString;
        public virtual string ChooseFirstLanguage => _defaultString;

        // List Backup Menu
        public virtual string ListBackupMenuTitle => _defaultString;

        // Backup Detail Menu
        public virtual string BackupNameMenuTitle => _defaultString;
        public virtual string BackupName => _defaultString;
        public virtual string BackupNameTitle => _defaultString;
        public virtual string BackupSourcePath => _defaultString;
        public virtual string BackupSourcePathTitle => _defaultString;
        public virtual string BackupTargetPath => _defaultString;
        public virtual string BackupTargetPathTitle => _defaultString;
        public virtual string BackupType => _defaultString;
        public virtual string BackupTypeTitle => _defaultString;

        // Execute Backup Menu
        public virtual string ExeBackupMenuTitle => _defaultString;
        public virtual string BackupNames => _defaultString;
        public virtual string ExeSelected => _defaultString;
        public virtual string ExeBackupInstruction => _defaultString;

        // Execute Backup Menu Details
        public virtual string ExeBackupMenuDetailTitle => _defaultString;
        public virtual string ExeBackupInProgress => _defaultString;
        public virtual string ExeBackupCompleted => _defaultString;

        // Change Log Format Menu
        public virtual string LogFormatMenuTitle => _defaultString;
        public virtual string LogFormat1 => _defaultString;
        public virtual string LogFormat2 => _defaultString;
        public virtual string LogFormatChanged => _defaultString;
        public virtual string CurrentLogFormat => _defaultString;

        // Settings menu
        public virtual string SettingsMenuTitle => _defaultString;
        public virtual string SettingsMenuBusiness => _defaultString;
        public virtual string SettingsMenuLanguage => _defaultString;
        public virtual string SettingsMenuLogFormat => _defaultString;
        public virtual string SettingsMenuExemple => _defaultString;

        // Message Box
        public virtual string MessageBoxInfoTitle => _defaultString;
        public virtual string MessageBoxJobCreated => _defaultString;
        public virtual string MessageBoxJobPaused => _defaultString;
        public virtual string MessageBoxJobExecuted => _defaultString;
        public virtual string MessageBoxJobStopped => _defaultString;
        public virtual string MessageBoxJobDeleted => _defaultString;
        public virtual string MessageBoxJobEdited => _defaultString;
        public virtual string MessageBoxSettingsSaved => _defaultString;
        public virtual string MessageBoxOk => _defaultString;
    }
}