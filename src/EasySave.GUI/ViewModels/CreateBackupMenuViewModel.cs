using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace EasySave.GUI.ViewModels;

public partial class CreateBackupMenuViewModel : ViewModelBase
{
    // Inputs
    [ObservableProperty] private string? backupName;
    [ObservableProperty] private string? sourcePath;
    [ObservableProperty] private string? targetPath;
    [ObservableProperty] private int selectedType;
    
    // Commands
    public ICommand ExitCommand { get; }
    public ICommand CreateBackupCommand { get; }
    public ICommand SetFullTypeCommand { get; }
    public ICommand SetDifferentialTypeCommand { get; }

    
    // String to display
    public string Title { get; }
    public string AskName { get; }
    public string AskSource { get; }
    public string AskTarget { get; }
    public string AskType { get; }
    public string Confirm { get; }
    public string BrowseFile { get; }
    public string WaterMarkBackupName { get; }
    public string WaterMarkBackupSourcePath { get; }
    public string WaterMarkBackupTargetPath { get; }
    public string FullType { get; }
    public string DifferentialType { get; }

    public CreateBackupMenuViewModel(MainWindowViewModel mainWindow) : base(mainWindow)
    {
        Title = Texts.CreateBackupMenuTitle;
        AskName = Texts.EnterBackupName;
        AskSource = Texts.EnterSourcePath;
        AskTarget = Texts.EnterTargetPath;
        AskType = Texts.EnterBackupType;
        Confirm = Texts.Confirm;
        BrowseFile = Texts.BrowseFile;
        WaterMarkBackupName = Texts.WaterMarkBackupName;
        WaterMarkBackupSourcePath = Texts.WaterMarkBackupSourcePath;
        WaterMarkBackupTargetPath = Texts.WaterMarkBackupTargetPath;
        FullType = Texts.Full;
        DifferentialType = Texts.Differential;

        SetFullTypeCommand = new RelayCommand(() => SelectedType = 1);
        SetDifferentialTypeCommand = new RelayCommand(() => SelectedType = 0);
        CreateBackupCommand = new RelayCommand(CreateBackup);

        ExitCommand = new RelayCommand(NavigateToBase);
    }

    private void CreateBackup()
    {
        BackupAppService.CreateBackup(backupName, sourcePath, targetPath, selectedType);
    }
    
}