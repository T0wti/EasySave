namespace EasySave.Application.Resources;

public class FrenchTextProviderConsole : FrenchTextProvider
{ 
    // Base menu
    public override string MainMenuTitle => FormatBigTitle(base.MainMenuTitle);
    public override string CreateBackup => Format(base.CreateBackup);
    public override string LanguageOption => Format(base.LanguageOption);
    public override string ExitOption => Format(base.ExitOption);
    public override string AskEntryFromUser => base.AskEntryFromUser;
    public override string EditBackup => Format(base.EditBackup);
    public override string DeleteBackup => Format(base.DeleteBackup);
    public override string WrongInput => base.WrongInput;
    public override string ListBackup => Format(base.ListBackup);
    public override string ExeBackup => Format(base.ExeBackup);
    public override string LogFormat => Format(base.LogFormat);

    // Creation BackupMenu 

    public override string EnterBackupName => base.EnterBackupName;
    public override string EnterSourcePath => base.EnterSourcePath;
    public override string EnterTargetPath => base.EnterTargetPath;
    public override string EnterBackupType => base.EnterBackupType;

    //
    public override string BackupCreated => base.BackupCreated;
    public override string BackupEdited => base.BackupEdited;
    public override string EnterBackupToDelete => base.EnterBackupToDelete;
    public override string BackupDeleted => base.BackupDeleted;


    public override string AskIdToEdit => base.AskIdToEdit;

    // Language menu
    public override string LanguageMenuTitle => FormatTitle(base.LanguageMenuTitle);
    public override string Language1 => Format(Format(base.Language1));
    public override string Language2 => Format(Format(base.Language2));
    
    // Choose first language menu
    public override string ChooseFirstLanguageMenuTitle => FormatTitle(base.ChooseFirstLanguageMenuTitle);
    public override string ChooseFirstLanguage => Format(base.ChooseFirstLanguage);
    
    // List Backup Menu
    public override string ListBackupMenuTitle => FormatTitle(base.ListBackupMenuTitle);
    
    // Backup Detail Menu
    public override string BackupNameMenuTitle => FormatTitle(base.BackupNameMenuTitle);
    public override string BackupName => Format(base.BackupName); // mettre la fonction pour sortir le nom d'une backup
    public override string BackupSourcePath => Format(base.BackupSourcePath);
    public override string BackupTargetPath => Format(base.BackupTargetPath);
    public override string BackupType => Format(base.BackupType);

    // Execute Backup Menu
    public override string ExeBackupMenuTitle => FormatTitle(base.ExeBackupMenuTitle);
    //public override string BackupNames => Format("nom save"); // not used for the moment
    public override string ExeBackupInstruction => Format(base.ExeBackupInstruction);
    
    // Execute Backup Menu Details
    public override string ExeBackupMenuDetailTitle => FormatTitle(base.ExeBackupMenuDetailTitle);
    public override string ExeBackupInProgress => Format(base.ExeBackupInProgress);
    public override string ExeBackupCompleted => Format(base.ExeBackupCompleted);

    // Change Log Format Menu
    public override string LogFormatMenuTitle => FormatTitle(base.LogFormatMenuTitle);
    public override string LogFormat1 => Format(base.LogFormat1);
    public override string LogFormat2 => Format(base.LogFormat2);
    public override string LogFormatChanged => Format(base.LogFormatChanged);
    public override string CurrentLogFormat => Format(base.CurrentLogFormat);

}