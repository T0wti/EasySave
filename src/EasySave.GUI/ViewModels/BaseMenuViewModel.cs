using System.Windows.Input;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.Input;
using EasySave.Domain.Services;

namespace EasySave.GUI.ViewModels;

public partial class BaseMenuViewModel : ViewModelBase
{
    public ICommand NavigateToSettingsCommand { get; }
    public ICommand NavigateToListBackupCommand { get; }
    public ICommand NavigateToCreateBackupCommand { get; }
    public ICommand NavigateToExecuteBackupCommand { get; }
    public ICommand ExitCommand { get; }

    public string Title { get; }
    public string CreateBackup { get; }
    public string DeleteBackup { get; }
    public string EditBackup { get; }
    public string ListBackup { get; }
    public string ExeBackup { get; }
    public string Settings { get; }
    public string Exit { get; }
    
    public BaseMenuViewModel(MainWindowViewModel mainWindow) : base(mainWindow)
    {
        Title = Texts.MainMenuTitle;
        CreateBackup = Texts.CreateBackup;
        DeleteBackup = Texts.DeleteBackup;
        EditBackup = Texts.EditBackup;
        ListBackup = Texts.ListBackup;
        ExeBackup = Texts.ExeBackup;
        Settings = Texts.SettingsMenu;
        Exit = Texts.Exit;

        NavigateToSettingsCommand = new RelayCommand(() =>
        {
            NavigateTo(new SettingsMenuViewModel(mainWindow));
        });
        NavigateToListBackupCommand = new RelayCommand(() =>
        {
            NavigateTo(new ListBackupMenuViewModel(mainWindow));
        });
        //NavigateToCreateBackupCommand = new RelayCommand(() =>
        //{
        //    NavigateTo(new CreateBackupMenuViewModel(mainWindow, new DialogService()));
        //});
        NavigateToExecuteBackupCommand = new RelayCommand(() =>
        {
            NavigateTo(new ExecuteBackupMenuViewModel(mainWindow));
        });
        ExitCommand = new RelayCommand(OnExit);
    }

    private void OnExit()
    {
        if (Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Shutdown();
        }
    }

}