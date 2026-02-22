using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasySave.Application.Exceptions;
using EasySave.Application.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata;
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

            // object as parameter means that if empty, it pauses all jobs. If jobId, pauses specific job
            PauseSelectedCommand = new AsyncRelayCommand<object>(PauseJobAsync);
            // AsyncRelayCommandOptions.AllowConcurrentExecutions => tells ExecuteSelectedCommand (or other) not to freeze the interface
            // Needed to allow clicking on the same button again without it being disabled
            ExecuteSelectedCommand = new AsyncRelayCommand<int>(ExecuteJobAsync, AsyncRelayCommandOptions.AllowConcurrentExecutions);
            ExecuteAllJobsCommand = new AsyncRelayCommand(ExecuteAllJobs, AsyncRelayCommandOptions.AllowConcurrentExecutions);
            StopSelectedCommand = new AsyncRelayCommand<object>(StopJobAsync);
        }

        private async Task PauseJobAsync(object? parameter)
        {
            IsThereError = false;
            try
            {
                // If parameter has a jobId, pause specific job
                if (parameter is int jobId) {
                    await Task.Run(() => BackupAppService.PauseBackup(jobId));
                    var job = GetJobViewModel(jobId);
                    if (job != null) job.IsProcessing = false; // To turn the play button clickable
                    Message = job.Job.Name + "\n" + Texts.MessageBoxJobPaused; // Message for single job
                }
                // If parameter empty, then pause all jobs
                else
                {
                    await Task.Run(() => BackupAppService.PauseAll());
                    foreach (var job in BackupJobs) job.IsProcessing = false; // To turn the play button clickable for each job
                    Message = Texts.MessageBoxAllJobsPaused; // Message for all jobs
                }

                IsMessageToDisplay = true;
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
        }


        private async Task ExecuteJobAsync(int jobId)
        {
            // Get job for progressbar
            var job = GetJobViewModel(jobId);
            if (job == null) return;

            //Check if job had been paused and resumes it if true
            if (BackupAppService.IsJobPaused(jobId))
            {
                job.IsProcessing = true;
                BackupAppService.ResumeBackup(jobId);

                StartProgressMonitoring(job);

                return; // To avoid continuing execution from 0
            }

            bool isSuccess = false; // To check if stopped by button or stopped by failure

            try
            {
                IsRunning = true;
                job.IsProcessing = true; // To avoid clicking on multiple buttons at the same time
                job.IsCompleted = false; // To get a green progress bar once done

                StartProgressMonitoring(job);

                BusinessSoftwareHasError = false;
                ErrorMessage = null;
                IsThereError = false;

                //await BackupAppService.ExecuteBackup(jobId);
                // Replaced by bellow to avoid freezing when pausing or stopping:
                await Task.Run(() => BackupAppService.ExecuteBackup(jobId));

                isSuccess = true;
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
                job.IsProcessing = false;
                IsRunning = false;
                IsExecutionDone = true;

                if (isSuccess)
                {
                    job.ProgressValue = 100; // When job is done
                    Progress = 100;
                    job.IsCompleted = true;
                    IsMessageToDisplay = true;
                    Message = job.Job.Name + "\n" + Texts.MessageBoxJobExecuted;
                }
                else
                {
                    Progress = 0;
                    job.ProgressValue = 0;
                }
            }
        }

        private async Task ExecuteAllJobs()
        {
            var selectedIds = BackupJobs
                .Select(x => x.Job.Id)
                .ToList();

            if (!selectedIds.Any())
                return;

            var tasks = selectedIds.Select(id => ExecuteJobAsync(id));
            
            await Task.WhenAll(tasks);
        }
        

        //TODO: Correct when stopping, progress bar jumps to 100%
        private async Task StopJobAsync(object? parameter)
        {
            IsThereError = false;
            try
            {
                // If parameter has a jobId, stop specific job
                if (parameter is int jobId)
                {
                    await Task.Run(() => BackupAppService.StopBackup(jobId));
                    var job = GetJobViewModel(jobId);
                    if (job != null)
                    {
                        job.IsProcessing = false; // To turn the play button clickable
                        job.ProgressValue = 0; // Turns back progress bar to 0
                        job.IsCompleted = false;
                        Message = job.Job.Name + "\n" + Texts.MessageBoxJobStopped; // Message for single job
                    }
                }
                // If parameter empty, then stop all jobs
                else
                {
                    await Task.Run(() => BackupAppService.StopAll());
                    foreach (var job in BackupJobs)
                    {
                        job.IsProcessing = false; // To turn the play button clickable for each job
                        job.ProgressValue = 0; // Turns back progress bar to 0 for each job
                        job.IsCompleted = false;
                    }
                    Message = Texts.MessageBoxAllJobsStopped; // Message for all jobs
                }

                IsMessageToDisplay = true;
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
        }

        // On searchbar text changed
        partial void OnSearchTextChanged(string value)
        {
            FilterJobs();
        }

        private void FilterJobs()
        {
            BackupJobs.Clear();

            var filteredJobs = _jobs.Where(j => j.Job.MatchesSearch(SearchText)).ToList();

            foreach (var job in filteredJobs)
            {
                BackupJobs.Add(job);
            }
        }

        private BackupJobSelectionViewModel? GetJobViewModel(int jobId)
        {
            return BackupJobs.FirstOrDefault(x => x.Job.Id == jobId);
        }

        private void StartProgressMonitoring(BackupJobSelectionViewModel job)
        {
            // _ is a trash variable when a function needs to...
            // ...return to a variable but you don't need it
            _ = Task.Run(async () =>
            {
                while (job.IsProcessing)
                {
                    var progressDto = BackupAppService.GetProgress(job.Job.Id);

                    if (progressDto != null) job.ProgressValue = progressDto.Progression;
                    await Task.Delay(250);
                }
            });
        }
    }
}