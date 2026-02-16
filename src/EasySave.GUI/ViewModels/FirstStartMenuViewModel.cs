using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasySave.Application.Resources;

namespace EasySave.GUI.ViewModels;

public partial class FirstStartMenuViewModel: ViewModelBase
{
    // To get states for radio buttons
    // ObservableProperty automatically generates public property of the same name (PasclCase)
    [ObservableProperty] private bool _isLanguage1Selected;
    [ObservableProperty] private bool _isLanguage2Selected;
    
    // Commands 
    public ICommand Language1Command { get; }
    public ICommand Language2Command { get; }
    
    // Texts providers
    private ITextProvider _french = new FrenchTextProvider();
    private ITextProvider _english = new EnglishTextProvider();
    
    // String to display
    public string Title { get; }
    public string Language1 { get; }
    public string Language2 { get; }
    public string Exit { get; }

    public FirstStartMenuViewModel(MainWindowViewModel mainWindow) : base(mainWindow)
    {
        Title = _english.ChooseFirstLanguageMenuTitle;
        Language1 = _french.Language1;
        Language2 = _english.Language2;
        
        Exit = Texts.Exit;
        
        // Handle the language change
        Language1Command = new RelayCommand(() =>
        {
            ChangeLanguage(new FrenchTextProvider());
        });
        Language2Command = new RelayCommand(() =>
        {
            ChangeLanguage(new EnglishTextProvider());
        });
    }
}