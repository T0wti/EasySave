using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace EasySave.Application.ViewModels;

public partial class CreateBackupMenuViewModel : ViewModelBase
{
    // Inputs
    [ObservableProperty] private string? backupName;
    [ObservableProperty] private string? sourcePath;
    [ObservableProperty] private string? targetPath;
    [ObservableProperty] private string? selectedType;
    
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

        SetFullTypeCommand = new RelayCommand(() => SelectedType = "Full");
        SetDifferentialTypeCommand = new RelayCommand(() => SelectedType = "Differential");
        CreateBackupCommand = new RelayCommand(CreateBackup);

        ExitCommand = new RelayCommand(NavigateToBase);
    }

    private void CreateBackup()
    {
        // code
    }
    
}