using CommunityToolkit.Mvvm.ComponentModel;
using EasySave.Application.Controllers;
using EasySave.Application.Resources;

namespace EasySave.Application.ViewModels;

public abstract class ViewModelBase : ObservableObject
{
    protected ITextProvider Texts;
    
    private readonly BackupController _backupController;
    private readonly ConfigurationController _configController;
    
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
    }

    protected void NavigateTo(ViewModelBase viewModel)
    {
        MainWindow.CurrentView = viewModel;
    }

    protected void NavigateToBase()
    {
        MainWindow.CurrentView = new BaseMenuViewModel(MainWindow);
    }
}