using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasySave.Application.DTOs;
using EasySave.GUI.Views;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EasySave.GUI.ViewModels
{
    public partial class DeleteBackupMenuViewModel : ViewModelBase
    {
        public ICommand DeleteBackupCommand { get; }

        public ObservableCollection<BackupJobDTO> BackupJobs { get; }

        [ObservableProperty]
        private BackupJobDTO _selectedJob;

        public string Title {  get; }
        public string Exit {  get; }
        public string Delete {  get; }

        public ICommand ExitCommand { get; }

        public DeleteBackupMenuViewModel(MainWindowViewModel mainWindow) : base(mainWindow)
        {
            Title = Texts.DeleteBackupTitle;
            Exit = Texts.Exit;
            Delete = Texts.Confirm;


            BackupJobs = new ObservableCollection<BackupJobDTO>(BackupAppService.GetAll());

            DeleteBackupCommand = new AsyncRelayCommand(DeleteBackup);

            ExitCommand = new RelayCommand(NavigateToBase);
        }

        private async Task DeleteBackup()
        {
            if (_selectedJob != null) //To avoid crash if no job selected
            {
                BackupAppService.DeleteBackup(SelectedJob.Id);
                BackupJobs.Remove(SelectedJob);

                await ShowMessageAsync(Texts.MessageBoxInfoTitle, Texts.MessageBoxJobDeleted, Texts.MessageBoxOk, false);
            }
        }
    }
}
