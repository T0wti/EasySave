using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using EasySave.Application.DTOs;

namespace EasySave.Application.ViewModels;

public partial class ListBackupMenuViewModel : ViewModelBase
{
    // Jobs
    
    // Command
    public ICommand ExitCommand { get; }
    
    // String 
    public string Title { get; }
    public string Exit { get; }

    public ListBackupMenuViewModel(MainWindowViewModel mainWindow) : base(mainWindow)
    {
        Title = Texts.ListBackupMenuTitle;
        Exit = Texts.Exit;

        ExitCommand = new RelayCommand(NavigateToBase);
    }
}