namespace EasySave.Application.Resources
{

    public class FrenchTextProvider : GeneralTextProvider
    {
        // Base menu
        public override string MainMenuTitle => "Menu principal";
        public override string CreateBackup => "1. Créer une sauvegarde de fichiers";
        public override string LanguageOption => "9. Changer la langue de la console";
        public override string ExitOption => "0. Quitter";
        public override string AskEntryFromUser => "Entrez le chiffre ou la commande souhaité : ";
        public override string EditBackup => "3. Modifier une sauvegarde de fichiers";
        public override string DeleteBackup => "2. Supprimer une sauvegarde de fichiers";
        public override string WrongInput => "Entrée invalide, veuillez  réessayez : ";
        public override string Exit => "Quitter";
        public override string Home => "Accueil";
        public override string Confirm => "Confirmer";
        public override string Save => "Sauvegarder";
        public override string ListBackup => "4. Lister les sauvegardes de fichiers";
        public override string ExeBackup => "5. Exécuter une sauvegarde de fichiers";
        public override string LogFormat => "8. Changer le format des logs";
        public override string SettingsMenu => "Réglages";

        // Home Page
        public override string HomeWelcome => "Bienvenue";
        public override string HomeConfiguredBackup => "Sauvegardes configurées";

        // Creation BackupMenu 
        public override string CreateBackupMenuTitle => "Créer une sauvegarde de fichiers";
        public override string EnterBackupName => "Veuillez entrer le nom de votre travail : ";
        public override string EnterSourcePath => "Veuillez entrer la source du dossier à sauvegarder (C:\\\\Users\\\\Downloads) : ";
        public override string EnterTargetPath => "Veuillez entrer la destination où sauvegarder le dossier (C:\\\\Users\\\\Save) :";
        public override string EnterBackupType => "Veuillez entrer le type de sauvegarder que vous voulez faire (1 - Complète | 2 - Différentielle) : ";
        public override string BrowseFile => "Explorer";
        public override string WaterMarkBackupName => "Nom de la sauvegarde";
        public override string WaterMarkBackupSourcePath => "Dossier source";
        public override string WaterMarkBackupTargetPath => "Dossier destination";
        public override string Full => "Complète";
        public override string Differential => "Différentielle";
        public override string ExtensionToEncryptTitle => "Extension à chiffrer";
        public override string Encrypt => "Chiffrer";

        //
        public override string BackupCreated => "Le travail de sauvegarde a été créé avec succès.";
        public override string BackupEdited => "Le travail de sauvegarde a été modifié avec succès";
        public override string EnterBackupToDelete => "Entrez l'ID du travail de sauvegarde à supprimer : ";
        public override string BackupDeleted => "Le travail de sauvegarde a été supprimé avec succès.";

        public override string AskIdToEdit => "Entrez l'ID du travail de sauvegarde à modifier :";


        // Language menu
        public override string LanguageMenuTitle => "Changer la langue";
        public override string Language1 => "1. Français";
        public override string Language2 => "2. Anglais";

        // Choose first language menu
        public override string ChooseFirstLanguageMenuTitle => "Choix de la langue par défaut";
        public override string ChooseFirstLanguage => "Choisissez la langue par défaut :";

        // List Backup Menu
        public override string ListBackupMenuTitle => "Ensembles des sauvegardes paramétrées";

        // Backup Detail Menu
        public override string BackupNameMenuTitle => "Détail de la sauvegarde de fichiers";
        public override string BackupName => "id-a-" + ". " + "nom-job"; // mettre la fonction pour sortir le nom d'une backup
        public override string BackupSourcePath => "";
        public override string BackupTargetPath => "";
        public override string BackupType => "";

        // Execute Backup Menu
        public override string ExeBackupMenuTitle => "Choix de la sauvegarde à exécuter";
        //public override string BackupNames => Format("nom save"); // not used for the moment
        public override string ExeBackupInstruction => "Tapez id_Sauvegarde";

        // Execute Backup Menu Details
        public override string ExeBackupMenuDetailTitle => "État de la sauvegarde";
        public override string ExeBackupInProgress => "Sauvegarde en cours...";
        public override string ExeBackupCompleted => "Sauvegarde terminée.";

        // Change Log Format Menu
        public override string LogFormatMenuTitle => "Changer le format des logs";
        public override string LogFormat1 => "1. JSON";
        public override string LogFormat2 => "2. XML";
        public override string LogFormatChanged => "Format de log modifié avec succès !";
        public override string CurrentLogFormat => "Format actuel : ";

        // Settings menu
        public override string SettingsMenuTitle => "Paramètres d'Easy Save";
        public override string SettingsMenuBusiness => "Logiciel suspendant l'exécution";
        public override string SettingsMenuLanguage => "Langue";
        public override string SettingsMenuLogFormat => "Format des fichiers de logs";
        public override string SettingsMenuExemple => "Exemple : Calculatrice.exe";

        // Message Box
        public override string MessageBoxInfoTitle => "Information";
        public override string MessageBoxJobDeleted => "Travail supprimé avec succès";
        public override string MessageBoxOk => "Ok";
    }
}