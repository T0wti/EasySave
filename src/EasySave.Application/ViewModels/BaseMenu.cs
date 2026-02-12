using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace EasySave.Application.ViewModels;

public partial class BaseMenu : ViewModelBase
{
    public ICommand NavigateToSettingsCommand { get; }

    public string Title { get; }
    public string CreateBackup { get; }
    public string DeleteBackup { get; }
    public string EditBackup { get; }
    public string ListBackup { get; }
    public string ExeBackup { get; }
    public string Settings { get; }
    public string Exit { get; }
    
    public BaseMenu(MainWindowViewModel mainWindow) : base(mainWindow)
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
            NavigateTo(new SettingsMenu(mainWindow));
        });
    }

}