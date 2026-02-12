namespace EasySave.Application.ViewModels;

public partial class SettingsMenu : ViewModelBase
{
    public string Title { get; }
    public string Language1 { get; }
    public string Language2 { get; }
    public string LogFormat1 { get; }
    public string LogFormat2 { get; }
    
    public SettingsMenu(MainWindowViewModel mainWindow) : base(mainWindow)
    {
        Title = Texts.SettingsMenuTitle;
        Language1 = Texts.Language1;
        Language2 = Texts.Language2;
        LogFormat1 = Texts.LogFormat1;
        LogFormat2 = Texts.LogFormat2;
    }
}