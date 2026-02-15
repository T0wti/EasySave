using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasySave.Domain.Services;
using EasySave.GUI.Services;
using System.Windows.Input;

namespace EasySave.GUI.ViewModels;

public partial class MainWindowViewModel : ObservableObject
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

    // Commands
    public ICommand NavigateToBaseMenuCommand { get; }
    public ICommand NavigateToCreateBackupCommand { get; }
    public ICommand NavigateToDeleteBackupCommand { get; }
    public ICommand NavigateToEditBackupCommand { get; }
    public ICommand NavigateToListBackupCommand { get; }
    public ICommand NavigateToExecuteBackupCommand { get; }
    public ICommand NavigateToSettingsCommand { get; }
    public ICommand ExitCommand { get; }

    public MainWindowViewModel()
    {
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

    private void OnExit()
    {
        if (Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Shutdown();
        }
    }
}