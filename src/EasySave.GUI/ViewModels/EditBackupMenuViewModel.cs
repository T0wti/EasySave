using CommunityToolkit.Mvvm.Input;
using EasySave.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace EasySave.GUI.ViewModels
{
    public partial class EditBackupMenuViewModel : ViewModelBase
    {
        // Jobs
        public ObservableCollection<BackupJobDTO> BackupJobs { get; }
        private BackupJobDTO _selectedJob;

        public BackupJobDTO SelectedJob
        {
            get => _selectedJob;
            set
            {
                if (SetProperty(ref _selectedJob, value) && value != null)
                {
                    NavigateTo(new EditBackupDetailMenuViewModel(MainWindow, value));
                }
            }
        }

        public string Title { get; }
        public string Exit {  get; }

        public ICommand ExitCommand { get; }

        public EditBackupMenuViewModel(MainWindowViewModel mainWindow) : base(mainWindow)
        {
            //Title = Texts.EditBackupMenuTitle;
            Title = "Edit Backup";
            Exit = Texts.Exit;

            BackupJobs = new ObservableCollection<BackupJobDTO>(jobs);

            ExitCommand = new RelayCommand(NavigateToBase);
        }
    }
}
