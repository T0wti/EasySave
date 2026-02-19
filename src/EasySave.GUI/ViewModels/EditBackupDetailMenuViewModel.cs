using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasySave.Application.DTOs;
using EasySave.GUI.Services;
using System.Threading.Tasks;
using System.Windows.Input;
using EasySave.Application.Exceptions;

namespace EasySave.GUI.ViewModels
{
    public partial class EditBackupDetailMenuViewModel : ViewModelBase
    {
        // File browser
        private readonly DialogService _dialogService;

        // Command
        public ICommand NavigateBackCommand { get; }
        public ICommand EditBackupCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand SetFullTypeCommand { get; }
        public ICommand SetDifferentialTypeCommand { get; }
        public ICommand BrowseSourceCommand { get; }
        public ICommand BrowseTargetCommand { get; }


        // Int
        private int _jobId;

        // ObservableProperties: to be able to be modified
        [ObservableProperty] private string _backupName;
        [ObservableProperty] private string _backupSourcePath;
        [ObservableProperty] private string _backupTargetPath;
        [ObservableProperty] private int selectedType;

        [ObservableProperty] private bool _isFullType;
        [ObservableProperty] private bool _isDifferentialType;
        [ObservableProperty] private bool _isThereError;
        [ObservableProperty] private string _errorMessage;
        
        [ObservableProperty] private bool _nameHasError;
        [ObservableProperty] private bool _sourceHasError;
        [ObservableProperty] private bool _targetHasError;

        // String
        public string Title { get; }
        public string AskName { get; }
        public string AskSource { get; }
        public string AskTarget { get; }
        public string AskType { get; }
        public string FullType { get; }
        public string DifferentialType { get; }
        public string BrowseFile { get; }
        public string Confirm { get; }
        public string Type { get; }

        public string Exit { get; }
        public bool IsFullTypeBase { get; }
        public bool IsDifferentialTypeBase { get; }

        public EditBackupDetailMenuViewModel(MainWindowViewModel mainWindow, BackupJobDTO selectedJob, DialogService dialogService) : base(mainWindow)
        {
            _dialogService = dialogService;
            Title = Texts.BackupNameMenuTitle;
            BackupName = selectedJob.Name;
            BackupSourcePath = selectedJob.SourcePath;
            BackupTargetPath = selectedJob.TargetPath;
            FullType = Texts.Full;
            DifferentialType = Texts.Differential;
            BrowseFile = Texts.BrowseFile;
            AskName = Texts.EnterBackupName;
            AskSource = Texts.EnterSourcePath;
            AskTarget = Texts.EnterTargetPath;
            AskType = Texts.EnterBackupType;
            Confirm = Texts.Confirm;
            Exit = Texts.Exit;

            _jobId = selectedJob.Id;
            Type = selectedJob.Type;

            SetFullTypeCommand = new RelayCommand(() => SelectedType = 1);
            SetDifferentialTypeCommand = new RelayCommand(() => SelectedType = 0);
            EditBackupCommand = new AsyncRelayCommand(EditBackup);

            BrowseSourceCommand = new AsyncRelayCommand(BrowseSourceAsync);
            BrowseTargetCommand = new AsyncRelayCommand(BrowseTargetAsync);

            ExitCommand = new RelayCommand(() => NavigateTo(new EditBackupMenuViewModel(mainWindow)));
            
            switch (Type)
            {
                case "Full":
                    IsFullTypeBase = true;
                    IsDifferentialTypeBase = false;
                    break;
                case "Differential":
                    IsFullTypeBase = false;
                    IsDifferentialTypeBase = true;
                    break;
                default:
                    IsFullTypeBase = false;
                    IsDifferentialTypeBase = false;
                    break;
            }
        }

        private async Task EditBackup()
        {
            try
            {
                BackupAppService.EditBackup(_jobId, BackupName, BackupSourcePath, BackupTargetPath, SelectedType);
                await ShowMessageAsync(Texts.MessageBoxJobEdited, "", "", Texts.MessageBoxOk, false, false);
            }
            catch (AppException e)
            {
                IsThereError = true;
                switch (e.ErrorCode)
                {
                    case AppErrorCode.NameEmpty:
                        NameHasError = true;
                        ErrorMessage = Texts.NameEmpty;
                        break;
                    case AppErrorCode.NameTooLong:
                        NameHasError = true;
                        ErrorMessage = Texts.NameTooLong;
                        break;

                    case AppErrorCode.SourcePathEmpty:
                        SourceHasError = true;
                        ErrorMessage = Texts.SourcePathEmpty;
                        break;
                    case AppErrorCode.SourcePathNotAbsolute:
                        SourceHasError = true;
                        ErrorMessage = Texts.SourcePathNotAbsolute;
                        break;
                    case AppErrorCode.SourcePathNotFound:
                        SourceHasError = true;
                        ErrorMessage = Texts.SourcePathNotFound;
                        break;

                    case AppErrorCode.TargetPathEmpty:
                        TargetHasError = true;
                        ErrorMessage = Texts.TargetPathEmpty;
                        break;
                    case AppErrorCode.TargetPathNotAbsolute:
                        TargetHasError = true;
                        ErrorMessage = Texts.TargetPathNotAbsolute;
                        break;
                    case AppErrorCode.targetPathNotFound:
                        TargetHasError = true;
                        ErrorMessage = Texts.TargetPathNotFound;
                        break;

                    case AppErrorCode.SourceEqualsTarget:
                        SourceHasError = true;
                        TargetHasError = true;
                        ErrorMessage = Texts.SourceEqualsTarget;
                        break;

                    default:
                        ErrorMessage = "";
                        break;
                }
                //Console.WriteLine(ErrorMessage); // à changer par l'affichage dans la pop-up
                //await ShowMessageAsync(Texts.MessageBoxInfoTitle, ErrorMessage, Texts.MessageBoxOk);
            }
            
        }
        private async Task BrowseSourceAsync()
        {
            var path = await _dialogService.OpenFolderPickerAsync();

            if (path != null) BackupSourcePath = path;
        }

        private async Task BrowseTargetAsync()
        {
            var path = await _dialogService.OpenFolderPickerAsync();

            if (path != null) BackupTargetPath = path;
        }

        partial void OnBackupNameChanged(string value)
        {
            NameHasError = false;
            ResetErrorStates();
        }
        partial void OnBackupSourcePathChanged(string value)
        {
            SourceHasError = false;
            ResetErrorStates();
        }
        partial void OnBackupTargetPathChanged(string value)
        {
            TargetHasError = false;
            ResetErrorStates();
        }

        private void ResetErrorStates()
        {
            IsThereError = false;
        }
    }
}
