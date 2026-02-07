namespace EasySave.Console.Resources;

public interface ITextProvider
{
    string Header { get; }
    string Footer { get; }
    string MainMenuTitle { get; }
    string AskEntryFromUser { get; }
    string ExitOption { get; }
    string WrongInput { get; }
    
    // Options in the base menu | Argan
    string DeleteBackup { get; }
    string CreateBackup { get; }
    string EditBackup { get; }
    string LanguageOption { get; }
    string ListBackup { get; }
    string ExeBackup { get; }
    // Options for the creation 

    string EnterBackupName { get; }
    string EnterSourcePath { get; }
    string EnterTargetPath { get; }
    string EnterBackupType { get; }

    // 

    string BackupCreated { get; }
    string EnterBackupToDelete { get; }
    string BackupDeleted { get; }

    // Options in the change language menu | Le
    string LanguageMenuTitle { get; }
    string Language1  { get; }
    string Language2 { get; }
    
    // Options in the first start menu | Loup
    string ChooseFirstLanguageMenuTitle { get; }
    string ChooseFirstLanguage { get; }
    
    // List Backup Menu
    string ListBackupMenuTitle { get; }
    
    // Backup Detail Menu
    string BackupNameMenuTitle { get; }
    string BackupName { get; }
    string BackupSourcePath { get; }
    string BackupTargetPath { get; }
    string BackupType { get; }
    
    // Execute Backup Menu
    string ExeBackupMenuTitle { get; }
    string BackupNames { get; }
    string ExeBackupInstruction { get; }
}




