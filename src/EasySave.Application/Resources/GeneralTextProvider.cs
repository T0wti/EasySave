namespace EasySave.Application.Resources;

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
        public virtual string LanguageOption => _defaultString;
        public virtual string ExitOption => _defaultString;
        public virtual string AskEntryFromUser => _defaultString;
        public virtual string DeleteBackup => _defaultString;
        public virtual string EditBackup => _defaultString;
        public virtual string WrongInput => _defaultString;
        public virtual string Exit => _defaultString;
        public virtual string Confirm => _defaultString;
        public virtual string ListBackup => _defaultString;
        public virtual string ExeBackup => _defaultString;
        public virtual string LogFormat => _defaultString;
        public virtual string SettingsMenu => _defaultString;

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

    //
        public virtual string BackupCreated => _defaultString;
        public virtual string BackupEdited => _defaultString;
        public virtual string EnterBackupToDelete => _defaultString;
        public virtual string BackupDeleted => _defaultString;
        public virtual string AskIdToEdit => _defaultString;


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
        public virtual string BackupSourcePath => _defaultString;
        public virtual string BackupTargetPath => _defaultString;
        public virtual string BackupType => _defaultString;
        
        // Execute Backup Menu
        public virtual string ExeBackupMenuTitle => _defaultString;
        public virtual string BackupNames => _defaultString;
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
    }