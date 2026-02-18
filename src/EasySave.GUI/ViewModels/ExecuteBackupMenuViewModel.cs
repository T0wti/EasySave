using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using EasySave.Application.DTOs;

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

        public ExecuteBackupMenuViewModel(MainWindowViewModel mainWindow) : base(mainWindow)
        {
            Title = Texts.ExeBackupMenuTitle;
            ExeSelected = Texts.ExeSelected;
            Exit = Texts.Exit;
            
            var jobs = BackupAppService.GetAll()
                .Select(dto => new BackupJobSelectionViewModel(dto));
            
            BackupJobs = new ObservableCollection<BackupJobSelectionViewModel>(jobs);

            ExitCommand = new RelayCommand(NavigateToBase);
            ExecuteSelectedCommand = new RelayCommand(ExecuteSelectedJobs);
        }

        private void ExecuteSelectedJobs()
        {
            var jobsToExecute = BackupJobs.Where(x => x.IsSelected).ToList();

            foreach (var selection in jobsToExecute)
            {
                BackupAppService.ExecuteBackup(selection.Job.Id);
            }

            //await ShowMessageAsync(Texts.MessageBoxInfoTitle, ErrorMessage, Texts.MessageBoxOk);
        }
    }
}