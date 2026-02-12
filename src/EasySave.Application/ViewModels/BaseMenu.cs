namespace EasySave.Application.ViewModels;

public partial class BaseMenu : ViewModelBase
{
    public string Title { get; }
    public string CreateBackup { get; }
    public string DeleteBackup { get; }
    public string EditBackup { get; }
    public string ListBackup { get; }
    public string ExeBackup { get; }
    public string LogFormat { get; }
    public string LanguageOption { get; }
    
    public BaseMenu()
    {
        Title = Texts.MainMenuTitle;
        CreateBackup = Texts.CreateBackup;
        DeleteBackup = Texts.DeleteBackup;
        EditBackup = Texts.EditBackup;
        ListBackup = Texts.ListBackup;
        ExeBackup = Texts.ExeBackup;
        LogFormat = Texts.LogFormat;
        LanguageOption = Texts.LanguageOption;
    }

}