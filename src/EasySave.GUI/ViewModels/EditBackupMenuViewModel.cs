using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasySave.Application.DTOs;
using EasySave.Application.Utils;
using EasySave.GUI.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace EasySave.GUI.ViewModels
{
    public partial class EditBackupMenuViewModel : ViewModelBase
    {
        // Jobs
        public ObservableCollection<BackupJobDTO> BackupJobs { get; }
        private BackupJobDTO _selectedJob;
        private readonly List<BackupJobDTO> _allJobs; // For searchbar

        [ObservableProperty] private string _searchText = string.Empty; // For searchbar

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
        public string Watermark {  get; }

        public ICommand ExitCommand { get; }

        public EditBackupMenuViewModel(MainWindowViewModel mainWindow) : base(mainWindow)
        {
            Title = Texts.EditBackupTitle;
            Exit = Texts.Exit;
            Watermark = Texts.ExeBackupSearchBarWatermark;

            _allJobs = BackupAppService.GetAll().ToList(); // For searchbar
            BackupJobs = new ObservableCollection<BackupJobDTO>(BackupAppService.GetAll());

            ExitCommand = new RelayCommand(NavigateToBase);
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
