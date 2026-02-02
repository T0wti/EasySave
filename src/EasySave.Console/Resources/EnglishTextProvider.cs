namespace EasySave.Console.Resources;

public class EnglishTextProvider : GeneralTextProvider
{
    // Base menu
    public override string MainMenuTitle => Format("------- Main Menu -------");
    public override string CreateBackup => Format("1. Create a file backup");
    public override string LanguageOption => Format("9. Change console language");
    public override string ExitOption => Format("0. Exit");
    public override string AskEntryFromUser => "Enter the desired number: ";
    public override string EditBackup => Format("3. Edit a file backup");
    public override string DeleteBackup => Format("2. Delete a file backup");

}