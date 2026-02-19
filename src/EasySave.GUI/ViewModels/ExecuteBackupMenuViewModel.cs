using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasySave.Application.DTOs;
using EasySave.Application.Exceptions;
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

        // Commands
        public ICommand ExitCommand { get; }
        public ICommand ExecuteSelectedCommand { get; }

        // Strings
        public string Title { get; }
        public string ExeSelected { get; }
        public string Exit { get; }

        // Inputs
        [ObservableProperty] private string? _errorMessage;
        [ObservableProperty] private bool _businessSoftwareHasError;
        [ObservableProperty] private bool _isRunning;

        public ExecuteBackupMenuViewModel(MainWindowViewModel mainWindow) : base(mainWindow)
        {
            Title = Texts.ExeBackupMenuTitle;
            ExeSelected = Texts.ExeSelected;
            Exit = Texts.Exit;

            var jobs = BackupAppService.GetAll()
                .Select(dto => new BackupJobSelectionViewModel(dto));

            BackupJobs = new ObservableCollection<BackupJobSelectionViewModel>(jobs);

            ExitCommand = new RelayCommand(NavigateToBase);

            // AsyncRelayCommand handles async naturally and disables the button while running
            ExecuteSelectedCommand = new AsyncRelayCommand(ExecuteSelectedJobsAsync);
        }

        private async Task ExecuteSelectedJobsAsync()
        {
            try
            {
                IsRunning = true;
                BusinessSoftwareHasError = false;
                ErrorMessage = null;

                var selectedIds = BackupJobs
                    .Where(x => x.IsSelected)
                    .Select(x => x.Job.Id)
                    .ToList();

                if (!selectedIds.Any())
                    return;

                // All selected jobs run in parallel
                await BackupAppService.ExecuteMultipleAsync(selectedIds);

                await ShowMessageAsync(
                    Texts.MessageBoxInfoTitle,
                    Texts.MessageBoxJobExecuted,
                    Texts.MessageBoxOk,
                    false);
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
                await ShowMessageAsync(
                    Texts.MessageBoxInfoTitle,
                    ErrorMessage,
                    Texts.MessageBoxOk,
                    true);
            }
            finally
            {
                IsRunning = false;
            }
        }
    }
}