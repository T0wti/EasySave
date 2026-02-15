using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasySave.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace EasySave.GUI.ViewModels
{
    public partial class EditBackupDetailMenuViewModel : ViewModelBase
    {
        // Command
        public ICommand NavigateBackCommand { get; }
        public ICommand ExitCommand { get; }

        // String
        public string Title { get; }
        public string BackupName { get; }
        public string BackupSourcePath { get; }
        public string BackupTargetPath { get; }
        public string BackupType { get; }
        public string Exit { get; }

        public EditBackupDetailMenuViewModel(MainWindowViewModel mainWindow, BackupJobDTO selectedJob) : base(mainWindow)
        {
            Title = Texts.BackupNameMenuTitle;
            BackupName = selectedJob.Name;
            BackupSourcePath = selectedJob.SourcePath;
            BackupTargetPath = selectedJob.TargetPath;
            BackupType = selectedJob.Type;
            Exit = Texts.Exit;

            ExitCommand = new RelayCommand(() => NavigateTo(new EditBackupMenuViewModel(mainWindow)));
        }
    }
}
