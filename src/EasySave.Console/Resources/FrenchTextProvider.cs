namespace EasySave.Console.Resources;

public class FrenchTextProvider: GeneralTextProvider
{
    // Base menu
    public override  string MainMenuTitle => Format("----- Menu principal -----");
    public override  string CreateBackup => Format( "1. Créer une sauvegarde de fichiers");
    public override  string LanguageOption => Format("9. Changer la langue de la console");
    public override  string ExitOption => Format("0. Quitter");
    public override string AskEntryFromUser =>"Entrez le chiffre souhaité : ";
    public override string EditBackup => Format("3. Modifier une sauvegarde de fichiers");
    public override string DeleteBackup => Format("2. Supprimer une sauvegarde de fichiers");
    public override string WrongInput => "Entrée invalide, veuillez  réessayez : ";
    public override string ListBackup => Format("4. Lister les sauvegardes de fichiers");
    public override string ExeBackup => Format("5. Exécuter une sauvegarde de fichiers");


    // Language menu
    public override string LanguageMenuTitle => Format("-- Changer la langue --");
    public override string Language1 => Format("1. Français");
    public override string Language2 => Format("2. Anglais");

    // Choose first language menu
    public override string ChooseFirstLanguageMenuTitle => Format("-- Choix de la langue par défaut --");
    public override string ChooseFirstLanguage => Format("Choisissez la langue par défaut :");
}