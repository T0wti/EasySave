using EasySave.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace EasySave.GUI.ViewModels
{
    public class DeleteBackupMenuViewModel : ViewModelBase
    {
        public ObservableCollection<BackupJobDTO> BackupJobs { get; }

        public string Title {  get; }
        public string Exit {  get; }
        public string Delete {  get; }

        public DeleteBackupMenuViewModel(MainWindowViewModel mainWindow) : base(mainWindow)
        {
            Title = Texts.DeleteBackup;
            Exit = Texts.Exit;
            Delete = Texts.Confirm;


            BackupJobs = new ObservableCollection<BackupJobDTO>(jobs);
        }
    }
}
