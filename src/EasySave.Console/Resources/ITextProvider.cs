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
    
    // Options in the change language menu | Le
    string LanguageMenuTitle { get; }
    string Language1  { get; }
    string Language2 { get; }
    
    // Options in the first start menu | Loup
    string ChooseFirstLanguageMenuTitle { get; }
    string ChooseFirstLanguage { get; }
}




