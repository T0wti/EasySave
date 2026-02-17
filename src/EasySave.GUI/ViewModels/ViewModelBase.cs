using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using EasySave.Application;
using EasySave.Application.DTOs;
using EasySave.Application.Resources;
using EasySave.GUI.Views;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasySave.GUI.ViewModels;

public abstract class ViewModelBase : ObservableObject
{
    protected ITextProvider Texts;
    protected readonly BackupAppService BackupAppService;
    protected readonly ConfigAppService ConfigAppService;
    protected MainWindowViewModel MainWindow { get; }

    public ViewModelBase(MainWindowViewModel mainWindow)
    {
        MainWindow = mainWindow;

        // Get the services by mainviewmodel (instead of recreate them)
        BackupAppService = mainWindow.BackupAppService;
        ConfigAppService = mainWindow.ConfigAppService;
        Texts = mainWindow.Texts;
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
        ConfigAppService.ChangeLanguage(code);
        MainWindow.RefreshTexts(language);
        NavigateTo(new SettingsMenuViewModel(MainWindow));
    }

    internal void ChangeLogFormat(int formatCode)
    {
        ConfigAppService.ChangeLogFormat(formatCode);
    }

    protected async Task ShowMessageAsync(string title, string message, string ok)
    {
        //Prepares message to display
        var messageBox = new MessageBoxWindow
        {
            DataContext = new MessageBoxViewModel(title, message, ok)
        };

        //Opens MessageBox
        if (Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            await messageBox.ShowDialog(desktop.MainWindow);
        }
    }
}