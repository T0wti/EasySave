using CommunityToolkit.Mvvm.ComponentModel;
using EasySave.Application.DTOs;

public partial class BackupJobSelectionViewModel : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
{
    public BackupJobDTO Job { get; }
    
    [ObservableProperty]
    private bool _isSelected;

    public BackupJobSelectionViewModel(BackupJobDTO job)
    {
        Job = job;
    }
}