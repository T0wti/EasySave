using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using EasySave.Application;
using EasySave.Application.Resources;
using EasySave.GUI.Views;
using EasySave.GUI.ViewModels;
using System.Linq;

namespace EasySave.GUI;

public partial class App : Avalonia.Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            DisableAvaloniaDataAnnotationValidation();

            // Créer les services une seule fois
            var backupAppService = AppServiceFactory.CreateBackupController();
            var configAppService = AppServiceFactory.CreateConfigurationController();

            // Bootstrap — config et clé
            if (!configAppService.FileExists())
                configAppService.EnsureConfigExists();
            configAppService.EnsureKeyExists();

            // Résoudre la langue
            var settings = configAppService.Load();
            ITextProvider texts = settings.LanguageCode == 0
                ? new FrenchTextProvider()
                : new EnglishTextProvider();

            // Créer le MainViewModel avec les services injectés
            var mainViewModel = new MainWindowViewModel(backupAppService, configAppService, texts);

            // Si premier démarrage, afficher le menu de langue
            if (settings.LanguageCode == -1) // ou une autre logique pour détecter le premier démarrage
                mainViewModel.CurrentView = new FirstStartMenuViewModel(mainViewModel);

            desktop.MainWindow = new MainWindow
            {
                DataContext = mainViewModel
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        foreach (var plugin in dataValidationPluginsToRemove)
            BindingPlugins.DataValidators.Remove(plugin);
    }
}