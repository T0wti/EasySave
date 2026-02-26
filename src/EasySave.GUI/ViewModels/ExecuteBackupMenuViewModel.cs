using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasySave.Application;
using EasySave.Application.Exceptions;
using EasySave.Application.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Threading; // Needed for timer

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
        [ObservableProperty] private bool _isThereError;
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

            IsThereError = false;
            IsExecutionDone = false;

            //_jobs = BackupAppService.GetAll()
            //        .Select(dto => new BackupJobSelectionViewModel(dto))
            //        .ToList();

            // Gets the jobs and the infos (to not restart from 0 when refreshing the page
            var allProgress = BackupAppService.GetAllProgress().ToList();
            var allJobs = BackupAppService.GetAll();
            var jobsForInterface = new List<BackupJobSelectionViewModel>(); // To be displayed on interface

            foreach (var job in allJobs)
            {
                var jobVm = new BackupJobSelectionViewModel(job, Texts);

                // Checks if current job already has a state saved
                var savedState = allProgress.FirstOrDefault(progress => progress.BackupJobId == job.Id);

                if (savedState != null)
                {
                    jobVm.ProgressValue = savedState.Progression;
                    jobVm.State = savedState.State; // To display state

                    switch (savedState.State)
                    {
                        case "Active":
                            jobVm.ProgressValue = savedState.Progression;
                            jobVm.IsProcessing = true;
                            //StartProgressMonitoring(jobVm); // Recheck progress
                            break;

                        case "Paused":
                            jobVm.ProgressValue = savedState.Progression;
                            jobVm.IsProcessing = false; // Play button enabled
                            break;

                        case "Completed":
                            jobVm.ProgressValue = 100;
                            jobVm.IsCompleted = true; // Green progressbar
                            jobVm.IsProcessing = false;
                            break;

                        case "Stopped":
                        case "Failed":
                        case "Inactive":
                        default:
                            jobVm.ProgressValue = 0;
                            jobVm.IsProcessing = false;
                            jobVm.IsCompleted = false;
                            break;
                    }
                }
                jobsForInterface.Add(jobVm);
            }
            _jobs = jobsForInterface;





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

            // For timer UpdateAllProgress()
            // DispatcherTimer is synchronised with the main UI thread
            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(250) };
            // Tick is every time the interval is done
            // s = sender and e = eventArgs
            // => lambda means every tick you do UpdateAllProgress()
            timer.Tick += (s, e) => UpdateAllProgress();
            // Begin timer
            timer.Start();
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
                }
                // If parameter empty, then pause all jobs
                else
                {
                    await Task.Run(() => BackupAppService.PauseAll());
                    foreach (var job in BackupJobs) job.IsProcessing = false; // To turn the play button clickable for each job
                }
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

                return; // To avoid continuing execution from 0
            }

            bool isSuccess = false; // To check if stopped by button or stopped by failure

            try
            {
                IsRunning = true;
                job.IsProcessing = true; // To avoid clicking on multiple buttons at the same time
                job.IsCompleted = false; // To get a green progress bar once done

                BusinessSoftwareHasError = false;
                ErrorMessage = null;
                IsThereError = false;

                //await BackupAppService.ExecuteBackup(jobId);
                // Replaced by bellow to avoid freezing when pausing or stopping:
                await Task.Run(() => BackupAppService.ExecuteBackup(jobId));

                // Checks if execution succeeded to avoid showing 100% when job stopped
                //isSuccess = true;
                var finalState = BackupAppService.GetProgress(jobId);
                isSuccess = (finalState != null && finalState.State == "Completed");
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
                }
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

        private void UpdateAllProgress()
        {
            var allProgress = BackupAppService.GetAllProgress().ToList();

            foreach (var job in BackupJobs)
            {
                var progressDto = allProgress.FirstOrDefault(p => p.BackupJobId == job.Job.Id);
                // To avoid null and potential crash
                if(progressDto != null)
                {
                    job.State = progressDto.State; // Update job state
                    job.CurrentFile = progressDto.CurrentSourceFile;
                }

                // Get the real state
                bool isPaused = BackupAppService.IsJobPaused(job.Job.Id);
                bool isRunning = BackupAppService.IsJobRunning(job.Job.Id);

                // If job is running or paused
                if (isRunning || isPaused)
                {
                    job.IsCompleted = false; // Turn back to blue from green !
                    job.IsProcessing = !isPaused; // Disables play if running or disables pause if paused

                    if (progressDto != null)
                    {
                        job.ProgressValue = progressDto.Progression;
                    }
                }
                // Job is stopped or failed
                else
                {
                    job.IsProcessing = false; // Enable play button

                    if (progressDto != null)
                    {
                        job.CurrentFile = string.Empty; // To remove the file if not running
                        // Job completed
                        if (progressDto.State == "Completed")
                        {
                            job.IsCompleted = true; // Turn the progress bar green
                            job.ProgressValue = 100;
                        }
                        else // "Stopped", "Failed", "Inactive"
                        {
                            job.IsCompleted = false;
                            job.ProgressValue = 0; // Make the progress bar go to 0
                        }
                    }
                }
            }
        }
    }
}