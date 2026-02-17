using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasySave.Application.Resources;
using EasySave.GUI.Services;
using System.Windows.Input;
using Avalonia.Media;

namespace EasySave.GUI.ViewModels
{

    public partial class MainWindowViewModel : ViewModelBase
    {
        private ViewModelBase _currentView;

        public ViewModelBase CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        // ObservableProperty automatically creates a public version of each private property below
        [ObservableProperty] private bool _isBaseActive;
        [ObservableProperty] private bool _isCreateActive;
        [ObservableProperty] private bool _isEditActive;
        [ObservableProperty] private bool _isDeleteActive;
        [ObservableProperty] private bool _isListActive;
        [ObservableProperty] private bool _isExecuteActive;
        [ObservableProperty] private bool _isSettingsActive;
        [ObservableProperty] private bool _isPaneOpen = true; // For burger menu

        // Commands
        public ICommand NavigateToBaseMenuCommand { get; }
        public ICommand NavigateToCreateBackupCommand { get; }
        public ICommand NavigateToDeleteBackupCommand { get; }
        public ICommand NavigateToEditBackupCommand { get; }
        public ICommand NavigateToListBackupCommand { get; }
        public ICommand NavigateToExecuteBackupCommand { get; }
        public ICommand NavigateToSettingsCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand TogglePaneCommand { get; } // Burger menu

        // Strings with [ObservableProperty] to update the burger menu instantly
        [ObservableProperty] private string _home;
        [ObservableProperty] private string _createBackup;
        [ObservableProperty] private string _deleteBackup;
        [ObservableProperty] private string _editBackup;
        [ObservableProperty] private string _listBackup;
        [ObservableProperty] private string _exeBackup;
        [ObservableProperty] private string _settings;
        [ObservableProperty] private string _exit;
        [ObservableProperty] private double _panelWidth;

        public MainWindowViewModel(MainWindowViewModel mainWindow) : base(mainWindow)
        {
            RefreshTexts(this.Texts);

            CurrentView = new BaseMenuViewModel(this);
            //CurrentView = new CreateBackupMenuViewModel(this, new DialogService());

            IsBaseActive = true;
            //IsCreateActive = true;

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
            Texts = newTexts; // To get the new language

            Home = Texts.Home;
            CreateBackup = Texts.CreateBackup;
            DeleteBackup = Texts.DeleteBackup;
            EditBackup = Texts.EditBackup;
            ListBackup = Texts.ListBackup;
            ExeBackup = Texts.ExeBackup;
            Settings = Texts.SettingsMenu;
            Exit = Texts.Exit;
            PanelWidth = CalculateOptimalPaneWidth();
        }

        private void OnExit()
        {
            if (Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.Shutdown();
            }
        }

        private double CalculateOptimalPaneWidth()
        {
            var menuTexts = new List<string>
        {
            Home, CreateBackup, DeleteBackup, EditBackup,
            ListBackup, ExeBackup, Settings, Exit
        };

            double maxTextWidth = 0;

            // Typeface by default on Windows
            var typeface = new Typeface("Segoe UI");
            double fontSize = 14; // Default Font Size

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

            // Icon width (45px) + spacing (20px) + text + marging (40px)
            double totalWidth = 45 + 20 + maxTextWidth + 40;

            // Limits
            return Math.Clamp(totalWidth, 200, 1000);
        }
    }
}