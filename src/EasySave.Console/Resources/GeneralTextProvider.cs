namespace EasySave.Console.Resources;

public class GeneralTextProvider : ITextProvider
{
    // Define an automatic indentation (to improve readability)
    private const string Indent = "       ";
    protected string Format(string text) => $"{Indent}{text}";

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
        public virtual string ListBackup => _defaultString;
        public virtual string ExeBackup => _defaultString;

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
        public virtual string BackupName => _defaultString;
        // Execute Backup Menu
        public virtual string ExeBackupMenuTitle => _defaultString;
        public virtual string BackupNames => _defaultString;
        public virtual string ExeBackupInstruction => _defaultString;
}