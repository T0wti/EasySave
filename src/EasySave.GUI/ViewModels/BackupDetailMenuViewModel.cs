using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using EasySave.Application.DTOs;

namespace EasySave.GUI.ViewModels
{

    public class BackupDetailMenuViewModel : ViewModelBase
    {
        // Command
        public ICommand ExitCommand { get; }

        // String
        public string Title { get; }
        public string BackupName { get; }
        public string BackupNameTitle { get; }
        public string BackupSourcePath { get; }
        public string BackupSourcePathTitle { get; }
        public string BackupTargetPath { get; }
        public string BackupTargetPathTitle { get; }
        public string BackupType { get; }
        public string BackupTypeTitle { get; }
        public string Exit { get; }

        public BackupDetailMenuViewModel(MainWindowViewModel mainWindow, BackupJobDTO job) : base(mainWindow)
        {
            Title = Texts.BackupNameMenuTitle;
            BackupName = job.Name;
            BackupNameTitle = Texts.BackupNameTitle;
            BackupSourcePath = job.SourcePath;
            BackupSourcePathTitle = Texts.BackupSourcePathTitle;
            BackupTargetPath = job.TargetPath;
            BackupTargetPathTitle = Texts.BackupTargetPathTitle;
            BackupType = job.Type;
            BackupTypeTitle = Texts.BackupTypeTitle;
            Exit = Texts.Exit;

            ExitCommand = new RelayCommand(() =>
            {
                NavigateTo(new ListBackupMenuViewModel(mainWindow));
            });

        }
    }
}