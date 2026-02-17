using System.Collections.Generic;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using EasySave.Application.DTOs;

namespace EasySave.GUI.ViewModels
{

    public class BackupJobsListViewModel : ObservableObject
    {
        private IEnumerable<BackupJobDTO> _jobs;
        private BackupJobDTO _selectedJob;

        public IEnumerable<BackupJobDTO> Jobs
        {
            get => _jobs;
            set => SetProperty(ref _jobs, value);
        }

        public BackupJobDTO SelectedJob
        {
            get => _selectedJob;
            set => SetProperty(ref _selectedJob, value);
        }

        public ICommand JobSelectedCommand { get; set; }

        public BackupJobsListViewModel()
        {
            _jobs = new List<BackupJobDTO>();
        }

    }
}