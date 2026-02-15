using CommunityToolkit.Mvvm.Input;
using EasySave.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace EasySave.GUI.ViewModels
{
    public partial class DeleteBackupMenuViewModel : ViewModelBase
    {

        // TO DO : GET TRUE IF A JOB IS SELECTED

        public ObservableCollection<BackupJobDTO> BackupJobs { get; }

        public string Title {  get; }
        public string Exit {  get; }
        public string Delete {  get; }

        public ICommand ExitCommand { get; }

        public DeleteBackupMenuViewModel(MainWindowViewModel mainWindow) : base(mainWindow)
        {
            Title = Texts.DeleteBackup;
            Exit = Texts.Exit;
            Delete = Texts.Confirm;


            BackupJobs = new ObservableCollection<BackupJobDTO>(jobs);

            ExitCommand = new RelayCommand(NavigateToBase);
        }
    }
}
