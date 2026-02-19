using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasySave.Application.Exceptions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EasySave.GUI.ViewModels
{
    public partial class ExecuteBackupMenuViewModel : ViewModelBase
    {
        // Use the wrapper collection
        public ObservableCollection<BackupJobSelectionViewModel> BackupJobs { get; }
        private readonly List<BackupJobSelectionViewModel> _jobs; // For searchbar

        // Commands
        public ICommand ExitCommand { get; }
        public ICommand PauseSelectedCommand { get; }
        public ICommand ExecuteSelectedCommand { get; }
        public ICommand ExecuteAllJobsCommand { get; }
        public ICommand StopSelectedCommand { get; }

        // Strings
        public string Title { get; }
        public string ExeSelected { get; }
        public string GeneralButtons { get; }
        public string Watermark { get; }
        public string Exit { get; }

        // Inputs
        [ObservableProperty] private string? _errorMessage;
        [ObservableProperty] private bool _businessSoftwareHasError;
        [ObservableProperty] private bool _isMessageToDisplay;
        [ObservableProperty] private bool _isThereError;
        [ObservableProperty] private string? _message;
        [ObservableProperty] private bool _isRunning;
        [ObservableProperty] private string _searchText; // For searchbar

        public ExecuteBackupMenuViewModel(MainWindowViewModel mainWindow) : base(mainWindow)
        {
            Title = Texts.ExeBackupMenuTitle;
            ExeSelected = Texts.ExeSelected;
            GeneralButtons = Texts.ExeBackupGeneralButtons;
            Watermark = Texts.ExeBackupSearchBarWatermark;
            Exit = Texts.Exit;

            IsMessageToDisplay = false;
            IsThereError = false;

            _jobs = BackupAppService.GetAll()
                    .Select(dto => new BackupJobSelectionViewModel(dto))
                    .ToList();

            BackupJobs = new ObservableCollection<BackupJobSelectionViewModel>(_jobs);

            SearchText = string.Empty;

            ExitCommand = new RelayCommand(NavigateToBase);
            PauseSelectedCommand = new AsyncRelayCommand(PauseSelectedJobs);
            ExecuteSelectedCommand = new AsyncRelayCommand<int>(ExecuteJobAsync);
            ExecuteAllJobsCommand = new AsyncRelayCommand(ExecuteAllJobsAsync);
            StopSelectedCommand = new AsyncRelayCommand(StopSelectedJobs);
        }

        private async Task PauseSelectedJobs()
        {
            IsThereError = false;
            IsMessageToDisplay = true;
            Message = Texts.MessageBoxJobPaused;
        }

        //TODO: Correct front button interaction
        private async Task ExecuteJobAsync(int jobId)
        {
            try
            {
                IsRunning = true;
                BusinessSoftwareHasError = false;
                ErrorMessage = null;
                IsThereError = false;
                await BackupAppService.ExecuteBackup(jobId);
                IsMessageToDisplay = true;
                Message = Texts.MessageBoxJobExecuted;
            }
            catch (AppException e)
            {
                switch (e.ErrorCode)
                {
                    case AppErrorCode.BusinessSoftwareRunning:
                        BusinessSoftwareHasError = true;
                        ErrorMessage = Texts.BusinessSoftwareRunning;
                        break;
                    default:
                        ErrorMessage = e.Message;
                        break;
                }

                await ShowMessageAsync(ErrorMessage, "", "", Texts.MessageBoxOk, true, false);
            }
            finally
            {
                IsRunning = false;
            }

        }

        private async Task ExecuteAllJobsAsync()
        {
            try
            {
                IsRunning = true;
                BusinessSoftwareHasError = false;
                ErrorMessage = null;
                IsThereError = false;

                var selectedIds = BackupJobs
                    .Select(x => x.Job.Id)
                    .ToList();

                if (!selectedIds.Any())
                    return;

                // All selected jobs run in parallel
                await BackupAppService.ExecuteMultiple(selectedIds);
                IsMessageToDisplay = true;
                Message = Texts.MessageBoxJobExecuted;
            }
            catch (AppException e)
            {
                switch (e.ErrorCode)
                {
                    case AppErrorCode.BusinessSoftwareRunning:
                        BusinessSoftwareHasError = true;
                        ErrorMessage = Texts.BusinessSoftwareRunning;
                        break;
                    default:
                        ErrorMessage = e.Message;
                        break;
                }

                await ShowMessageAsync(ErrorMessage, "", "", Texts.MessageBoxOk, true, false);
            }
            finally
            {
                IsRunning = false;
            }
        }

        private async Task StopSelectedJobs()
        {
            IsThereError = false;
            IsMessageToDisplay = true;
            Message = Texts.MessageBoxJobStopped;
        }

        // On searchbar text changed
        partial void OnSearchTextChanged(string value)
        {
            FilterJobs();
        }

        private void FilterJobs()
        {
            BackupJobs.Clear();

            // If searchbar empty, display all jobs
            if (string.IsNullOrEmpty(SearchText))
            {
                foreach (var job in _jobs)
                {
                    BackupJobs.Add(job);
                }
                return;
            }

            var filteredJobs = _jobs.Where(j =>
                // Filter by name
                j.Job.Name.ToLower().Contains(SearchText.ToLower()) ||
                // Filter by source
                j.Job.SourcePath.ToLower().Contains(SearchText.ToLower()) ||
                // Filter by target
                j.Job.TargetPath.ToLower().Contains(SearchText.ToLower())
            ).ToList();

            foreach (var job in filteredJobs)
            {
                BackupJobs.Add(job);
            }
        }
    }
}