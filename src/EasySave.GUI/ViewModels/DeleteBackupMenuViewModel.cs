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
        public string JobDeleted {  get; }
        public string Yes {  get; }
        public string No {  get; }
        public string Ok {  get; }
        public string DeleteConfirmation {  get; }

        public ICommand ExitCommand { get; }

        public DeleteBackupMenuViewModel(MainWindowViewModel mainWindow) : base(mainWindow)
        {
            Title = Texts.DeleteBackupTitle;
            Exit = Texts.Exit;
            Delete = Texts.MessageBoxDelete;
            JobDeleted = Texts.MessageBoxJobDeleted;
            Ok = Texts.MessageBoxOk;
            Yes = Texts.MessageBoxYes;
            No = Texts.MessageBoxNo;
            DeleteConfirmation = Texts.MessageBoxDeleteConfirmation;

            BackupJobs = new ObservableCollection<BackupJobDTO>(BackupAppService.GetAll());

            DeleteBackupCommand = new AsyncRelayCommand(DeleteBackup);

            ExitCommand = new RelayCommand(NavigateToBase);
        }

        private async Task DeleteBackup()
        {
            if (_selectedJob != null) //To avoid crash if no job selected
            {
                bool userConfirmed = await ShowMessageAsync(DeleteConfirmation, Yes, No, Ok, true, true);
                if (userConfirmed) {
                    BackupAppService.DeleteBackup(SelectedJob.Id);
                    BackupJobs.Remove(SelectedJob);
                    await ShowMessageAsync(JobDeleted, "", "", Ok, false, false);
                }
            }
        }
    }
}
