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
        public ICommand ExitCommand { get; }

        public EditBackupDetailMenuViewModel(MainWindowViewModel mainWindow, BackupJobDTO selectedJob) : base(mainWindow)
        {
            ExitCommand = new RelayCommand(() => NavigateTo(new EditBackupMenuViewModel(mainWindow)));
        }
    }
}
