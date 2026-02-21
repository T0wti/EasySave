using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using EasySave.Application;
using EasySave.Application.Resources;
using EasySave.GUI.Views;
using EasySave.GUI.ViewModels;
using System.Linq;
using Avalonia.Styling;

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

            // Create the services
            var backupAppService = AppServiceFactory.CreateBackupController();
            var configAppService = AppServiceFactory.CreateConfigurationController();


            var settings = configAppService.Load();
            ITextProvider texts = settings.LanguageCode == 0
                ? new FrenchTextProvider()
                : new EnglishTextProvider();

            // Theme mode
            switch (settings.ThemeCode)
            {
                case 1:
                    this.RequestedThemeVariant = ThemeVariant.Light;
                    break;
                case 2:
                    this.RequestedThemeVariant = ThemeVariant.Dark;
                    break;
                case 0:
                default:
                    this.RequestedThemeVariant = ThemeVariant.Default;
                    break;
            }

            // Create the MainViewModel with the services
            var mainViewModel = new MainWindowViewModel(backupAppService, configAppService, texts);

            if (!configAppService.FileExists())
            {
                configAppService.EnsureConfigExists();
                mainViewModel.CurrentView = new FirstStartMenuViewModel(mainViewModel);
            }

            configAppService.EnsureKeyExists();

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