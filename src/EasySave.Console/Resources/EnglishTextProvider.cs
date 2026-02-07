namespace EasySave.Console.Resources;

public class EnglishTextProvider : GeneralTextProvider
{
    // Base menu
    public override string MainMenuTitle => Format("------- Main Menu -------");
    public override string CreateBackup => Format("1. Create a file backup");
    public override string LanguageOption => Format("9. Change console language");
    public override string ExitOption => Format("0. Exit");
    public override string AskEntryFromUser => "Enter the desired number or command: ";
    public override string EditBackup => Format("3. Edit a file backup");
    public override string DeleteBackup => Format("2. Delete a file backup");
    public override string WrongInput => "Invalid input, please try again: ";
    public override string ListBackup => Format("4. List all file backups");
    public override string ExeBackup => Format("5. Execute a file backup");

    // Creation BackupMenu 

    public override string EnterBackupName => "Please enter the name of your backup job: ";
    public override string EnterSourcePath => "Please enter the source path of the folder to back up: ";
    public override string EnterTargetPath => "Please enter the destination path for the backup: ";
    public override string EnterBackupType => "Please enter the backup type (1 - Full | 2 - Differential): ";

    //
    public override string BackupCreated => "Backup job created successfully.";
    public override string BackupEdited => "Backup job edited successfully.";
    public override string EnterBackupToDelete => "Enter the ID of the backup job to delete: ";
    public override string BackupDeleted => "Backup job deleted successfully.";


    public override string AskIdToEdit => "Enter the ID of the backup job to edit:";

    // Language menu
    public override string LanguageMenuTitle => Format("-- Change the language --");
    public override string Language1 => Format("1. French");
    public override string Language2 => Format("2. English");
    
    // Choose first language menu
    public override string ChooseFirstLanguageMenuTitle => Format("-- Choose the default language --");
    public override string ChooseFirstLanguage => Format("Choose the default language :");
    
    // List Backup Menu
    public override string ListBackupMenuTitle => Format("-- All configured backups --");
    
    // Backup Detail Menu
    public override string BackupNameMenuTitle => Format("-- Files backup details --");
    public override string BackupName => Format("id-a-" + ". " + "nom-job"); // mettre la fonction pour sortir le nom d'une backup
    public override string BackupSourcePath => Format("");
    public override string BackupTargetPath => Format("");
    public override string BackupType => Format("");

    // Execute Backup Menu
    public override string ExeBackupMenuTitle => Format("-- Choose the file backup to execute --");
    public override string BackupNames => Format("nom save"); // modifier avec l'appel de fonction qui va lister le nom des sauvegardes lignes par lignes
    public override string ExeBackupInstruction => Format("Command exemple : id_Backup");
}