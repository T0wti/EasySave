using CommunityToolkit.Mvvm.Input;
using EasySave.Application.DTOs;
using EasySave.GUI.Services;
using System.Collections.ObjectModel;
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
                    var dialogService = new DialogService();
                    NavigateTo(new EditBackupDetailMenuViewModel(MainWindow, value, dialogService));
                }
            }
        }

        public string Title { get; }
        public string Exit {  get; }

        public ICommand ExitCommand { get; }

        public EditBackupMenuViewModel(MainWindowViewModel mainWindow) : base(mainWindow)
        {
            //Title = Texts.EditBackupMenuTitle; TODO
            Title = "Edit Backup";
            Exit = Texts.Exit;

            BackupJobs = new ObservableCollection<BackupJobDTO>(BackupAppService.GetAll());

            ExitCommand = new RelayCommand(NavigateToBase);
        }
    }
}
