using CommunityToolkit.Mvvm.ComponentModel;
using EasySave.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasySave.GUI.ViewModels
{
    public partial class BackupJobSelectionViewModel : ObservableObject
    {
        public BackupJobDTO Job { get; }
        [ObservableProperty] private double _progressValue;
        [ObservableProperty] private bool _isProcessing; // To avoid clicking on all buttons at the same time
        [ObservableProperty] private bool _isCompleted; // To make the progress bar turn green when backup is done

        public BackupJobSelectionViewModel(BackupJobDTO job) => Job = job;
        // Replaces :
        //public BackupJobSelectionViewModel(BackupJobDTO job)
        //{
        //    Job = job;
        //}
    }
}
