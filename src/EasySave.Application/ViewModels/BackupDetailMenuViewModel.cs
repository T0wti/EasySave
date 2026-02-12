using System.Windows.Input;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasySave.Application.DTOs;

namespace EasySave.Application.ViewModels;

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

    public BackupDetailMenuViewModel(MainWindowViewModel mainWindow, BackupJobDTO job) : base(mainWindow)
    {
        Title = Texts.BackupNameMenuTitle;
        BackupName = job.Name;
        BackupSourcePath = job.SourcePath;
        BackupTargetPath = job.TargetPath;
        BackupType = job.Type;

        NavigateBackCommand = new RelayCommand(() =>
        {
            // faire après
        });
    }
}