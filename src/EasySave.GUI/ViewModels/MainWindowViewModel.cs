using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasySave.Domain.Services;
using EasySave.GUI.Services;

namespace EasySave.GUI.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private ViewModelBase _currentView;

    public ViewModelBase CurrentView
    {
        get => _currentView;
        set => SetProperty(ref _currentView, value);
    }
    //ObservableProperty automatically creates a public version of each private property below
    [ObservableProperty] private bool _isBaseActive;
    [ObservableProperty] private bool _isCreateActive;
    [ObservableProperty] private bool _isEditActive;
    [ObservableProperty] private bool _isDeleteActive;
    [ObservableProperty] private bool _isListActive;
    [ObservableProperty] private bool _isExecuteActive;
    [ObservableProperty] private bool _isSettingsActive;

    public MainWindowViewModel()
    {
        //CurrentView = new BaseMenuViewModel(this);
        CurrentView = new CreateBackupMenuViewModel(this, new DialogService());
    }

    [RelayCommand]
    public void NavigateToBaseMenu()
    {
        CurrentView = new BaseMenuViewModel(this);
        ResetActiveStates();
        IsBaseActive = true;
    }

    [RelayCommand]
    public void NavigateToCreateBackup()
    {
        CurrentView = new CreateBackupMenuViewModel(this, new DialogService());
        ResetActiveStates();
        IsCreateActive = true;
    }

    [RelayCommand]
    public void NavigateToDeleteBackup()
    {
        CurrentView = new DeleteBackupMenuViewModel(this);
        ResetActiveStates();
        IsDeleteActive = true;
    }

    //[RelayCommand]
    //public void NavigateToEditBackup() => CurrentView = new EditBackupMenuViewModel(this);

    [RelayCommand]
    public void NavigateToListBackup()
    {
        CurrentView = new ListBackupMenuViewModel(this);
        ResetActiveStates();
        IsListActive = true;
    }

    [RelayCommand]
    public void NavigateToExecuteBackup()
    {
        CurrentView = new ExecuteBackupMenuViewModel(this);
        ResetActiveStates();
        IsExecuteActive = true;
    }

    [RelayCommand]
    public void NavigateToSettings()
    {
        CurrentView = new SettingsMenuViewModel(this);
        ResetActiveStates();
        IsSettingsActive = true;
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
}