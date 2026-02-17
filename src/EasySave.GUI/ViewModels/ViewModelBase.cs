using CommunityToolkit.Mvvm.ComponentModel;
using EasySave.Application;
using EasySave.Application.DTOs;
using EasySave.Application.Resources;
using System.Collections.Generic;

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

        // Récupère les services depuis MainWindow au lieu de les recréer
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
}