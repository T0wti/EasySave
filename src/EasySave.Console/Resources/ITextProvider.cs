namespace EasySave.Console.Resources;

public interface ITextProvider
{
    string Header { get; }
    string Footer { get; }
    string MainMenuTitle { get; }
    string AskEntryFromUser { get; }
    string ExitOption { get; }
    string WrongInput { get; }
    
    // Options in the base menu
    string DeleteBackup { get; }
    string CreateBackup { get; }
    string EditBackup { get; }
    string LanguageOption { get; }
    
    // Options in the change language menu
    string LanguageMenuTitle { get; }
    string Language1  { get; }
    string Language2 { get; }
    
    
}