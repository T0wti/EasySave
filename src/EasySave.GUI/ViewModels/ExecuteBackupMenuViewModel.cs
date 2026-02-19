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
        [ObservableProperty] private double _progress; // For progressbar
        [ObservableProperty] private bool _isExecutionDone; // For progressbar

        public ExecuteBackupMenuViewModel(MainWindowViewModel mainWindow) : base(mainWindow)
        {
            Title = Texts.ExeBackupMenuTitle;
            ExeSelected = Texts.ExeSelected;
            GeneralButtons = Texts.ExeBackupGeneralButtons;
            Watermark = Texts.ExeBackupSearchBarWatermark;
            Progress = 0;
            Exit = Texts.Exit;

            IsMessageToDisplay = false;
            IsThereError = false;
            IsExecutionDone = false;

            _jobs = BackupAppService.GetAll()
                    .Select(dto => new BackupJobSelectionViewModel(dto))
                    .ToList();

            BackupJobs = new ObservableCollection<BackupJobSelectionViewModel>(_jobs);

            SearchText = string.Empty;

            ExitCommand = new RelayCommand(NavigateToBase);
            PauseSelectedCommand = new AsyncRelayCommand(PauseSelectedJobs);
            // AsyncRelayCommandOptions.AllowConcurrentExecutions => tells ExecuteSelectedCommand not to freeze the interface
            // Needed to avoid clicking on all buttons at the same time
            ExecuteSelectedCommand = new AsyncRelayCommand<int>(ExecuteJobAsync, AsyncRelayCommandOptions.AllowConcurrentExecutions);
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
            // Get job for progressbar
            var job = BackupJobs.FirstOrDefault(x => x.Job.Id == jobId);
            if (job == null) return;

            try
            {
                IsRunning = true;
                job.IsProcessing = true; // TO avoid clicking on multiple buttons at the same time
                job.IsCompleted = false; // To get a green progress bar once done

                // _ is a trash variable when a function needs to
                // return to a variable but you don't need it
                _ = Task.Run(async () =>
                {
                    while (job.IsProcessing)
                    {
                        var progressDto = BackupAppService.GetProgress(jobId);

                        if(progressDto != null) job.ProgressValue = progressDto.Progression;
                        await Task.Delay(250);
                    }
                });

                BusinessSoftwareHasError = false;
                ErrorMessage = null;
                IsThereError = false;
                await BackupAppService.ExecuteBackup(jobId);
                job.ProgressValue = 100; // When job is done
                job.IsCompleted = true;

                IsMessageToDisplay = true;
                Message = job.Job.Name + "\n" + Texts.MessageBoxJobExecuted;
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
                IsExecutionDone = true;
                Progress = 100;
                job.IsProcessing = false;
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