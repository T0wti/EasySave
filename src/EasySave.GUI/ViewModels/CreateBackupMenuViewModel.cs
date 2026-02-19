using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasySave.Application.Exceptions;
using EasySave.GUI.Services;

namespace EasySave.GUI.ViewModels
{

    public partial class CreateBackupMenuViewModel : ViewModelBase
    {
        // File browser
        private readonly DialogService _dialogService;

        // Inputs
        [ObservableProperty] private string? backupName;
        [ObservableProperty] private string? sourcePath;
        [ObservableProperty] private string? targetPath;
        [ObservableProperty] private int selectedType;

        [ObservableProperty] private bool _isEncryptionEnabled;
        [ObservableProperty] private bool _isThereError;
        [ObservableProperty] private string? _errorMessage;
        
        [ObservableProperty] private bool _nameHasError;
        [ObservableProperty] private bool _sourceHasError;
        [ObservableProperty] private bool _targetHasError;

        // Commands
        public ICommand ExitCommand { get; }
        public ICommand CreateBackupCommand { get; }
        public ICommand SetFullTypeCommand { get; }
        public ICommand SetDifferentialTypeCommand { get; }
        public ICommand BrowseSourceCommand { get; }
        public ICommand BrowseTargetCommand { get; }


        // String to display
        public string Title { get; }
        public string AskName { get; }
        public string AskSource { get; }
        public string AskTarget { get; }
        public string AskType { get; }
        public string Confirm { get; }
        public string Exit { get; }
        public string BrowseFile { get; }
        public string WaterMarkBackupName { get; }
        public string WaterMarkBackupSourcePath { get; }
        public string WaterMarkBackupTargetPath { get; }
        public string FullType { get; }
        public string DifferentialType { get; }
        public string Encrypt { get; }

        public CreateBackupMenuViewModel(MainWindowViewModel mainWindow, DialogService dialogService) : base(mainWindow)
        {
            _dialogService = dialogService;
            Title = Texts.CreateBackupMenuTitle;
            AskName = Texts.EnterBackupName;
            AskSource = Texts.EnterSourcePath;
            AskTarget = Texts.EnterTargetPath;
            AskType = Texts.EnterBackupType;
            Confirm = Texts.Confirm;
            Exit = Texts.Exit;
            BrowseFile = Texts.BrowseFile;
            WaterMarkBackupName = Texts.WaterMarkBackupName;
            WaterMarkBackupSourcePath = Texts.WaterMarkBackupSourcePath;
            WaterMarkBackupTargetPath = Texts.WaterMarkBackupTargetPath;
            FullType = Texts.Full;
            DifferentialType = Texts.Differential;
            Encrypt = Texts.Encrypt;
            IsThereError = false;
            selectedType = 1;

            SetFullTypeCommand = new RelayCommand(() => SelectedType = 1);
            SetDifferentialTypeCommand = new RelayCommand(() => SelectedType = 0);
            CreateBackupCommand = new AsyncRelayCommand(CreateBackup);

            BrowseSourceCommand = new AsyncRelayCommand(BrowseSourceAsync);
            BrowseTargetCommand = new AsyncRelayCommand(BrowseTargetAsync);

            ExitCommand = new RelayCommand(NavigateToBase);
        }

        private async Task CreateBackup()
        {
            ResetErrorStates();
            try
            {
                BackupAppService.CreateBackup(backupName, sourcePath, targetPath, selectedType);
                await ShowMessageAsync(Texts.MessageBoxJobCreated, "", "", Texts.MessageBoxOk, false, false);
                NavigateToBase();
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

            if (path != null) SourcePath = path;
        }

        private async Task BrowseTargetAsync()
        {
            var path = await _dialogService.OpenFolderPickerAsync();

            if (path != null) TargetPath = path;
        }

        partial void OnBackupNameChanged(string value) {
            NameHasError = false;
            ResetErrorStates();
        }
        partial void OnSourcePathChanged(string value) {
            SourceHasError = false;
            ResetErrorStates();
        }
        partial void OnTargetPathChanged(string value) {
            TargetHasError = false;
            ResetErrorStates();
        }

        private void ResetErrorStates()
        {
            IsThereError = false;
        }
    }
}