using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasySave.Application.DTOs;
using EasySave.Application.Exceptions;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Tmds.DBus.Protocol;

namespace EasySave.GUI.ViewModels
{
    public partial class ExecuteBackupMenuViewModel : ViewModelBase
    {
        // Use the wrapper collection
        public ObservableCollection<BackupJobSelectionViewModel> BackupJobs { get; }

        // Commands
        public ICommand ExitCommand { get; }
        public ICommand PauseSelectedCommand { get; }
        public ICommand ExecuteSelectedCommand { get; }
        public ICommand StopSelectedCommand { get; }

        // Strings
        public string Title { get; }
        public string ExeSelected { get; }
        public string GeneralButtons { get; }
        public string Exit { get; }

        // Inputs
        [ObservableProperty] private string? _errorMessage;
        [ObservableProperty] private bool _businessSoftwareHasError;
        [ObservableProperty] private bool _isMessageToDisplay;
        [ObservableProperty] private bool _isThereError;
        [ObservableProperty] private string? _message;
        [ObservableProperty] private bool _isRunning;

        public ExecuteBackupMenuViewModel(MainWindowViewModel mainWindow) : base(mainWindow)
        {
            Title = Texts.ExeBackupMenuTitle;
            ExeSelected = Texts.ExeSelected;
            GeneralButtons = Texts.ExeBackupGeneralButtons;
            Exit = Texts.Exit;

            IsMessageToDisplay = false;
            IsThereError = false;

            var jobs = BackupAppService.GetAll()
                .Select(dto => new BackupJobSelectionViewModel(dto));

            BackupJobs = new ObservableCollection<BackupJobSelectionViewModel>(jobs);

            ExitCommand = new RelayCommand(NavigateToBase);
            PauseSelectedCommand = new AsyncRelayCommand(PauseSelectedJobs);
            ExecuteSelectedCommand = new AsyncRelayCommand(ExecuteSelectedJobsAsync);
            StopSelectedCommand = new AsyncRelayCommand(StopSelectedJobs);
        }

        private async Task PauseSelectedJobs()
        {
            IsThereError = false;
            IsMessageToDisplay = true;
            Message = Texts.MessageBoxJobPaused;
        }

        private async Task ExecuteSelectedJobsAsync()
        {
            try
            {
                IsRunning = true;
                BusinessSoftwareHasError = false;
                ErrorMessage = null;
                IsThereError = false;


                var selectedIds = BackupJobs
                    .Where(x => x.IsSelected)
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
    }
}