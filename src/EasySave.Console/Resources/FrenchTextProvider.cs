namespace EasySave.Console.Resources;

public class FrenchTextProvider: GeneralTextProvider
{
    // Base menu
    public override  string MainMenuTitle => Format("----- Menu principal -----");
    public override  string CreateBackup => Format( "1. Créer une sauvegarde de fichiers");
    public override  string LanguageOption => Format("9. Changer la langue de la console");
    public override  string ExitOption => Format("0. Quitter");
    public override string AskEntryFromUser =>"Entrez le chiffre ou la commande souhaité : ";
    public override string EditBackup => Format("3. Modifier une sauvegarde de fichiers");
    public override string DeleteBackup => Format("2. Supprimer une sauvegarde de fichiers");
    public override string WrongInput => "Entrée invalide, veuillez  réessayez : ";
    public override string ListBackup => Format("4. Lister les sauvegardes de fichiers");
    public override string ExeBackup => Format("5. Exécuter une sauvegarde de fichiers");

    // Creation BackupMenu 

    public override string EnterBackupName => "Veuillez entrer le nom de votre travail : ";
    public override string EnterSourcePath => "Veuillez entrer la source du dossier à sauvegarder : ";
    public override string EnterTargetPath => "Veuillez entrer la destination où sauvegarder le dossier ";
    public override string EnterBackupType => "Veuillez entrer le type de sauvegarder que vous voulez faire (1 - Full | 2 - Differential) : ";

    //
    public override string BackupCreated => "Le travail de sauvegarde a été créé avec succès.";
    public override string BackupEdited => "Le travail de sauvegarde a été modifié avec succès";
    public override string EnterBackupToDelete => "Entrez l'ID du travail de sauvegarde à supprimer : ";
    public override string BackupDeleted => "Le travail de sauvegarde a été supprimé avec succès.";

    public override string AskIdToEdit => "Entrez l'ID du travail de sauvegarde à modifier :";


    // Language menu
    public override string LanguageMenuTitle => Format("-- Changer la langue --");
    public override string Language1 => Format(Format("1. Français"));
    public override string Language2 => Format(Format("2. Anglais"));

    // Choose first language menu
    public override string ChooseFirstLanguageMenuTitle => Format("-- Choix de la langue par défaut --");
    public override string ChooseFirstLanguage => Format("Choisissez la langue par défaut :");
    
    // List Backup Menu
    public override string ListBackupMenuTitle => Format("-- Ensembles des sauvegardes paramétrées --");
    
    // Backup Detail Menu
    public override string BackupNameMenuTitle => Format("-- Détail de la sauvegarde de fichiers --");
    public override string BackupName => Format("id-a-" + ". " + "nom-job"); // mettre la fonction pour sortir le nom d'une backup
    public override string BackupSourcePath => Format("");
    public override string BackupTargetPath => Format("");
    public override string BackupType => Format("");

    // Execute Backup Menu
    public override string ExeBackupMenuTitle => Format("-- Choix de la sauvegarde à exécuter --");
    public override string BackupNames => Format("nom save"); // modifier avec l'appel de fonction qui va lister le nom des sauvegardes lignes par lignes
    public override string ExeBackupInstruction => Format("Tapez id_Sauvegarde");
    
    // Execute Backup Menu Details
    public override string ExeBackupMenuDetailTitle => Format("-- État de la sauvegarde --");
    public override string ExeBackupInProgress => Format("Sauvegarde en cours...");
    public override string ExeBackupCompleted => Format("Sauvegarde terminée.");
}