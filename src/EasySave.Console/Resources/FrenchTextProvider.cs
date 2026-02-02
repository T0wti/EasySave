namespace EasySave.Console.Resources;

public class FrenchTextProvider: GeneralTextProvider
{
    public override  string MainMenuTitle => Format("----- Menu principal -----");
    public override  string BackupOption => Format( "1. Sauvegarde des fichiers");
    public override  string LanguageOption => Format("2. Changer la langue de la console");
    public override  string ExitOption => Format("0. Quitter");
    public override string AskEntryFromUser =>"Entrez le chiffre souhaité : ";
}