using CommunityToolkit.Mvvm.ComponentModel;
using EasySave.Application.DTOs;
using EasySave.Application.Resources;

namespace EasySave.GUI.ViewModels
{
    public partial class BackupJobSelectionViewModel : ObservableObject
    {
        public BackupJobDTO Job { get; }
        private readonly ITextProvider _texts;

        [ObservableProperty] private double _progressValue;
        [ObservableProperty] private bool _isProcessing;
        [ObservableProperty] private bool _isCompleted;
        [ObservableProperty] private string _state;

        public string TypeLabel => Job.Type?.ToLower() switch
        {
            "full" => _texts.Full,
            "differential" => _texts.Differential,
            _ => Job.Type
        } ?? string.Empty;

        public BackupJobSelectionViewModel(BackupJobDTO job, ITextProvider texts)
        {
            Job = job;
            _texts = texts;
            _state = "Inactive";
        }
    }
}