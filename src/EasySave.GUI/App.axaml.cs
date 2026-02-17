using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using EasySave.Application;
using EasySave.Application.Utils;
using EasySave.GUI.ViewModels;
using EasySave.GUI.Views;
using System;
using System.Linq;

namespace EasySave.GUI
{

    public partial class App : Avalonia.Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            var backupAppService = AppServiceFactory.CreateBackupController();
            if (CommandRunner.TryRun(Environment.GetCommandLineArgs().Skip(1).ToArray(), backupAppService))
            {
                Environment.Exit(0);
                return;
            }

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
                // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
                DisableAvaloniaDataAnnotationValidation();

                var configAppService = AppServiceFactory.CreateConfigurationController();

                var mainViewModel = new MainWindowViewModel(null);

                if (!configAppService.FileExists())
                {
                    configAppService.EnsureConfigExists();
                    mainViewModel.CurrentView = new FirstStartMenuViewModel(mainViewModel);
                }
                desktop.MainWindow = new MainWindow
                {
                    DataContext = mainViewModel,
                };
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void DisableAvaloniaDataAnnotationValidation()
        {
            // Get an array of plugins to remove
            var dataValidationPluginsToRemove =
                BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

            // remove each entry found
            foreach (var plugin in dataValidationPluginsToRemove)
            {
                BindingPlugins.DataValidators.Remove(plugin);
            }
        }
    }
}