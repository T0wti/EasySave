using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using EasySave.Application.DTOs;

namespace EasySave.GUI.ViewModels
{

    public partial class ExecuteBackupMenuViewModel : ViewModelBase
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
                    BackupAppService.ExecuteBackup(_selectedJob.Id);
                }
            }
        }

        // Command
        public ICommand ExitCommand { get; }

        // String 
        public string Title { get; }
        public string Exit { get; }

        public ExecuteBackupMenuViewModel(MainWindowViewModel mainWindow) : base(mainWindow)
        {
            Title = Texts.ExeBackupMenuTitle;
            Exit = Texts.Exit;

            BackupJobs = new ObservableCollection<BackupJobDTO>(BackupAppService.GetAll());

            ExitCommand = new RelayCommand(NavigateToBase);
        }

    }
}