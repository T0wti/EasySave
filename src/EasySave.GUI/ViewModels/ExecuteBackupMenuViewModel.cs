using System.Collections.ObjectModel;
using System.Linq; // Nécessaire pour le .Select et .Where
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
        public string Exit { get; }

        public ExecuteBackupMenuViewModel(MainWindowViewModel mainWindow) : base(mainWindow)
        {
            Title = Texts.ExeBackupMenuTitle;
            Exit = Texts.Exit;

            // Transformation des DTOs en Wrappers lors du chargement
            var jobs = BackupAppService.GetAll()
                .Select(dto => new BackupJobSelectionViewModel(dto));
            
            BackupJobs = new ObservableCollection<BackupJobSelectionViewModel>(jobs);

            // Initialisation des commandes
            ExitCommand = new RelayCommand(NavigateToBase);
            ExecuteSelectedCommand = new RelayCommand(ExecuteSelectedJobs);
        }

        private void ExecuteSelectedJobs()
        {
            // On filtre pour ne garder que ceux dont IsSelected est true
            var jobsToExecute = BackupJobs.Where(x => x.IsSelected).ToList();

            foreach (var selection in jobsToExecute)
            {
                // On accède au DTO via la propriété .Job du wrapper
                BackupAppService.ExecuteBackup(selection.Job.Id);
            }
        }
    }
}