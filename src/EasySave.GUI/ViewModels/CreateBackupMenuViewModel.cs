using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

            SetFullTypeCommand = new RelayCommand(() => SelectedType = 1);
            SetDifferentialTypeCommand = new RelayCommand(() => SelectedType = 0);
            CreateBackupCommand = new RelayCommand(CreateBackup);

            BrowseSourceCommand = new AsyncRelayCommand(BrowseSourceAsync);
            BrowseTargetCommand = new AsyncRelayCommand(BrowseTargetAsync);

            ExitCommand = new RelayCommand(NavigateToBase);
        }

        private void CreateBackup()
        {
            BackupAppService.CreateBackup(backupName, sourcePath, targetPath, selectedType);
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
    }
}