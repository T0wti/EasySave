using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasySave.Application.DTOs;
using System.Threading.Tasks;
using System.Windows.Input;
using EasySave.GUI.Services;

namespace EasySave.GUI.ViewModels
{
    public partial class EditBackupDetailMenuViewModel : ViewModelBase
    {
        // Command
        public ICommand NavigateBackCommand { get; }
        public ICommand EditBackupCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand SetFullTypeCommand { get; }
        public ICommand SetDifferentialTypeCommand { get; }

        // Int
        private int _jobId;

        // ObservableProperties: to be able to be modified
        [ObservableProperty] private string _backupName;
        [ObservableProperty] private string _backupSourcePath;
        [ObservableProperty] private string _backupTargetPath;
        [ObservableProperty] private int selectedType;

        [ObservableProperty] private bool _isFullType;
        [ObservableProperty] private bool _isDifferentialType;

        // String
        public string Title { get; }
        public string AskType { get; }
        public string FullType { get; }
        public string DifferentialType { get; }
        public string Confirm { get; }
        public string Type { get; }

        public string Exit { get; }
        public bool IsFullTypeBase { get; }
        public bool IsDifferentialTypeBase { get; }

        public EditBackupDetailMenuViewModel(MainWindowViewModel mainWindow, BackupJobDTO selectedJob) : base(mainWindow)
        {
            Title = Texts.BackupNameMenuTitle;
            BackupName = selectedJob.Name;
            BackupSourcePath = selectedJob.SourcePath;
            BackupTargetPath = selectedJob.TargetPath;
            FullType = Texts.Full;
            DifferentialType = Texts.Differential;
            AskType = Texts.EnterBackupType;
            Confirm = Texts.Confirm;
            Exit = Texts.Exit;

            _jobId = selectedJob.Id;
            Type = selectedJob.Type;

            SetFullTypeCommand = new RelayCommand(() => SelectedType = 1);
            SetDifferentialTypeCommand = new RelayCommand(() => SelectedType = 0);
            EditBackupCommand = new AsyncRelayCommand(EditBackup);

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
            //TODO: modify backup type
            BackupAppService.EditBackup(_jobId, BackupName, BackupSourcePath, BackupTargetPath, 2);

            await ShowMessageAsync(Texts.MessageBoxInfoTitle, Texts.MessageBoxJobEdited, Texts.MessageBoxOk);
        }

    }
}
