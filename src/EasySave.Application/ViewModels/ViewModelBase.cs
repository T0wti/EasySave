using CommunityToolkit.Mvvm.ComponentModel;
using EasySave.Application.Controllers;
using EasySave.Application.DTOs;
using EasySave.Application.Resources;

namespace EasySave.Application.ViewModels;

public abstract class ViewModelBase : ObservableObject
{
    protected ITextProvider Texts;
    
    protected readonly BackupController _backupController;
    protected readonly ConfigurationController _configController;
    
    protected IEnumerable<BackupJobDTO> jobs;
    
    protected MainWindowViewModel MainWindow { get; private set; }

    public ViewModelBase(MainWindowViewModel mainWindow)
    {
        MainWindow = mainWindow;
        
        _backupController = ControllerFactory.CreateBackupController();
        _configController = ControllerFactory.CreateConfigurationController();
        
        if (!_configController.FileExists())
        {
            Texts = new EnglishTextProvider();
            _configController.EnsureConfigExists();
            // menu premier démarrage
        }

        var settings = _configController.Load();

        Texts = settings.LanguageCode == 0
            ? new FrenchTextProvider()
            : new EnglishTextProvider();
        jobs = _backupController.GetAll();
    }

    protected void NavigateTo(ViewModelBase viewModel)
    {
        MainWindow.CurrentView = viewModel;
    }

    protected void NavigateToBase()
    {
        MainWindow.CurrentView = new BaseMenuViewModel(MainWindow);
    }

    internal void ChangeLanguage(ITextProvider language)
    {
        Texts = language;

        int code = language is FrenchTextProvider ? 0 : 1;

        _configController.ChangeLanguage(code);
        NavigateTo(new SettingsMenuViewModel(MainWindow));
    }

    internal void ChangeLogFormat(int formatCode)
    {
        _configController.ChangeLogFormat(formatCode);
    }
}