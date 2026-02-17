using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using EasySave.Application.DTOs;

namespace EasySave.GUI.ViewModels
{

    public partial class ListBackupMenuViewModel : ViewModelBase
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
                    NavigateTo(new BackupDetailMenuViewModel(MainWindow, value));
                }
            }
        }

        // Command
        public ICommand ExitCommand { get; }

        // String 
        public string Title { get; }
        public string Exit { get; }

        public ListBackupMenuViewModel(MainWindowViewModel mainWindow) : base(mainWindow)
        {
            Title = Texts.ListBackupMenuTitle;
            Exit = Texts.Exit;

            BackupJobs = new ObservableCollection<BackupJobDTO>(jobs);

            ExitCommand = new RelayCommand(NavigateToBase);
        }
    }
}