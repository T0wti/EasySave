using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasySave.Application;
using EasySave.Application.Resources;
using EasySave.GUI.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Input;

namespace EasySave.GUI.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    // Services share with all the viewmodels
    public BackupAppService BackupAppService { get; }
    public ConfigAppService ConfigAppService { get; }
    public ITextProvider Texts { get; private set; }

    private ViewModelBase _currentView;
    public ViewModelBase CurrentView
    {
        get => _currentView;
        set => SetProperty(ref _currentView, value);
    }

    [ObservableProperty] private bool _isBaseActive;
    [ObservableProperty] private bool _isCreateActive;
    [ObservableProperty] private bool _isEditActive;
    [ObservableProperty] private bool _isDeleteActive;
    [ObservableProperty] private bool _isListActive;
    [ObservableProperty] private bool _isExecuteActive;
    [ObservableProperty] private bool _isSettingsActive;
    [ObservableProperty] private bool _isPaneOpen = true;

    public ICommand NavigateToBaseMenuCommand { get; }
    public ICommand NavigateToCreateBackupCommand { get; }
    public ICommand NavigateToDeleteBackupCommand { get; }
    public ICommand NavigateToEditBackupCommand { get; }
    public ICommand NavigateToListBackupCommand { get; }
    public ICommand NavigateToExecuteBackupCommand { get; }
    public ICommand NavigateToSettingsCommand { get; }
    public ICommand ExitCommand { get; }
    public ICommand TogglePaneCommand { get; }

    [ObservableProperty] private string _home;
    [ObservableProperty] private string _createBackup;
    [ObservableProperty] private string _deleteBackup;
    [ObservableProperty] private string _editBackup;
    [ObservableProperty] private string _listBackup;
    [ObservableProperty] private string _exeBackup;
    [ObservableProperty] private string _settings;
    [ObservableProperty] private string _exit;
    [ObservableProperty] private double _panelWidth;

    public MainWindowViewModel(
        BackupAppService backupAppService,
        ConfigAppService configAppService,
        ITextProvider texts)
    {
        BackupAppService = backupAppService;
        ConfigAppService = configAppService;
        Texts = texts;

        RefreshTexts(texts);

        CurrentView = new BaseMenuViewModel(this);
        IsBaseActive = true;

        NavigateToBaseMenuCommand = new RelayCommand(() =>
        {
            CurrentView = new BaseMenuViewModel(this);
            ResetActiveStates();
            IsBaseActive = true;
        });

        NavigateToCreateBackupCommand = new RelayCommand(() =>
        {
            CurrentView = new CreateBackupMenuViewModel(this, new DialogService());
            ResetActiveStates();
            IsCreateActive = true;
        });

        NavigateToDeleteBackupCommand = new RelayCommand(() =>
        {
            CurrentView = new DeleteBackupMenuViewModel(this);
            ResetActiveStates();
            IsDeleteActive = true;
        });

        NavigateToEditBackupCommand = new RelayCommand(() =>
        {
            CurrentView = new EditBackupMenuViewModel(this);
            ResetActiveStates();
            IsEditActive = true;
        });

        NavigateToListBackupCommand = new RelayCommand(() =>
        {
            CurrentView = new ListBackupMenuViewModel(this);
            ResetActiveStates();
            IsListActive = true;
        });

        NavigateToExecuteBackupCommand = new RelayCommand(() =>
        {
            CurrentView = new ExecuteBackupMenuViewModel(this);
            ResetActiveStates();
            IsExecuteActive = true;
        });

        NavigateToSettingsCommand = new RelayCommand(() =>
        {
            CurrentView = new SettingsMenuViewModel(this);
            ResetActiveStates();
            IsSettingsActive = true;
        });

        ExitCommand = new RelayCommand(OnExit);
        TogglePaneCommand = new RelayCommand(() => IsPaneOpen = !IsPaneOpen);
    }

    private void ResetActiveStates()
    {
        IsBaseActive = false;
        IsCreateActive = false;
        IsEditActive = false;
        IsDeleteActive = false;
        IsListActive = false;
        IsExecuteActive = false;
        IsSettingsActive = false;
    }

    public void RefreshTexts(ITextProvider newTexts)
    {
        Texts = newTexts;

        Home = Texts.Home;
        CreateBackup = Texts.CreateBackupTitle;
        DeleteBackup = Texts.DeleteBackupTitle;
        EditBackup = Texts.EditBackupTitle;
        ListBackup = Texts.ListBackupTitle;
        ExeBackup = Texts.ExeBackupTitle;
        Settings = Texts.SettingsMenuTitle;
        Exit = Texts.Exit;
        PanelWidth = CalculateOptimalPaneWidth();
    }

    private void OnExit()
    {
        if (Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.Shutdown();
    }

    private double CalculateOptimalPaneWidth()
    {
        var menuTexts = new List<string>
        {
            Home, CreateBackup, DeleteBackup, EditBackup,
            ListBackup, ExeBackup, Settings, Exit
        };

        double maxTextWidth = 0;

        var typeface = new Typeface("Segoe UI");
        double fontSize = 14;

        foreach (var text in menuTexts)
        {
            if (string.IsNullOrEmpty(text)) continue;

            var formattedText = new FormattedText(
                text,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                typeface,
                fontSize,
                Brushes.Black
            );

            maxTextWidth = Math.Max(maxTextWidth, formattedText.Width);
        }

        double totalWidth = 45 + 20 + maxTextWidth + 40;
        return Math.Clamp(totalWidth, 200, 1000);
    }
}