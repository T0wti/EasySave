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
}