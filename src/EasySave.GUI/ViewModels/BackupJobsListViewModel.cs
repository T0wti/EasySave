using CommunityToolkit.Mvvm.ComponentModel;
using EasySave.Application.DTOs;
using EasySave.Application.Resources;
using EasySave.Application.Utils;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace EasySave.GUI.ViewModels
{

    public partial class BackupJobsListViewModel : ObservableObject
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