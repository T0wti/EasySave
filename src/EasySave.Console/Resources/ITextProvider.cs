namespace EasySave.Console.Resources;

public interface ITextProvider
{
    string Header { get; }
    string Footer { get; }
    string MainMenuTitle { get; }
    string AskEntryFromUser { get; }
    
    // Options in the base menu
    string DeleteBackup { get; }
    string CreateBackup { get; }
    string EditBackup { get; }
    string LanguageOption { get; }
    string ExitOption { get; }
    
}