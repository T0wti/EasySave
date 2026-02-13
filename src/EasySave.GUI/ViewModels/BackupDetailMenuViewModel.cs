using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using EasySave.Application.DTOs;

namespace EasySave.GUI.ViewModels;

public class BackupDetailMenuViewModel : ViewModelBase
{
    // Command
    public ICommand NavigateBackCommand { get; }
    
    // String
    public string Title { get; }
    public string BackupName { get; }
    public string BackupSourcePath { get; }
    public string BackupTargetPath { get; }
    public string BackupType { get; }
    public string Exit { get; }

    public BackupDetailMenuViewModel(MainWindowViewModel mainWindow, BackupJobDTO job) : base(mainWindow)
    {
        Title = Texts.BackupNameMenuTitle;
        BackupName = job.Name;
        BackupSourcePath = job.SourcePath;
        BackupTargetPath = job.TargetPath;
        BackupType = job.Type;
        Exit = Texts.Exit;

        NavigateBackCommand = new RelayCommand(() =>
        {
            NavigateTo(new ListBackupMenuViewModel(mainWindow));
        });
    }
}