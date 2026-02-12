using CommunityToolkit.Mvvm.ComponentModel;
using EasySave.Application.Controllers;
using EasySave.Application.Resources;

namespace EasySave.Application.ViewModels;

public abstract class ViewModelBase : ObservableObject
{
    protected ITextProvider Texts;
    
    private readonly BackupController _backupController;
    private readonly ConfigurationController _configController;

    public ViewModelBase()
    {
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
}