using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasySave.Application.DTOs;
using EasySave.Application.Utils;
using EasySave.GUI.Views;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Linq;
using static System.Reflection.Metadata.BlobBuilder;

namespace EasySave.GUI.ViewModels
{
    public partial class DeleteBackupMenuViewModel : ViewModelBase
    {
        public ICommand DeleteBackupCommand { get; }

        public ObservableCollection<BackupJobDTO> BackupJobs { get; }
        private readonly List<BackupJobDTO> _allJobs; // For searchbar

        [ObservableProperty] private string _searchText = string.Empty; // For searchbar
        [ObservableProperty] private BackupJobDTO _selectedJob;

        public string Title {  get; }
        public string Exit {  get; }
        public string Delete {  get; }
        public string JobDeleted {  get; }
        public string Yes {  get; }
        public string No {  get; }
        public string Ok {  get; }
        public string Watermark { get; }
        public string DeleteConfirmation {  get; }

        public ICommand ExitCommand { get; }

        public DeleteBackupMenuViewModel(MainWindowViewModel mainWindow) : base(mainWindow)
        {
            Title = Texts.DeleteBackupTitle;
            Exit = Texts.Exit;
            Delete = Texts.MessageBoxDelete;
            JobDeleted = Texts.MessageBoxJobDeleted;
            Yes = Texts.MessageBoxYes;
            No = Texts.MessageBoxNo;
            Ok = Texts.MessageBoxOk;
            Watermark = Texts.ExeBackupSearchBarWatermark;
            DeleteConfirmation = Texts.MessageBoxDeleteConfirmation;

            _allJobs = BackupAppService.GetAll().ToList();

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
                    _allJobs.Remove(SelectedJob); // For searchbar
                    BackupJobs.Remove(SelectedJob);
                    await ShowMessageAsync(JobDeleted, "", "", Ok, false, false);
                }
            }
        }

        partial void OnSearchTextChanged(string value)
        {
            BackupJobs.Clear();
            var filtered = _allJobs.Where(j => j.MatchesSearch(value)).ToList();
            foreach (var job in filtered)
            {
                BackupJobs.Add(job);
            }
        }
    }
}
