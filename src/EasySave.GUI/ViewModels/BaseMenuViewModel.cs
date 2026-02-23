using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace EasySave.GUI.ViewModels
{

    public partial class BaseMenuViewModel : ViewModelBase
    {
        public ICommand NavigateToSettingsCommand { get; }
        public ICommand NavigateToListBackupCommand { get; }
        public ICommand NavigateToCreateBackupCommand { get; }
        public ICommand NavigateToExecuteBackupCommand { get; }
        public ICommand ExitCommand { get; }

        public string Title { get; }
        public string CreateBackup { get; }
        public string DeleteBackup { get; }
        public string EditBackup { get; }
        public string ListBackup { get; }
        public string ExeBackup { get; }
        public string Settings { get; }
        public string Exit { get; }
        public string Welcome { get; }
        public string ConfiguredBackupStr { get; }
        public int BackupNumber { get; }
        public string HomeTipTitle { get; }
        //[ObservableProperty] private string _homeRandomTip;
        public string HomeRandomTip { get; set; }

        public BaseMenuViewModel(MainWindowViewModel mainWindow) : base(mainWindow)
        {
            Title = Texts.MainMenuTitle;
            CreateBackup = Texts.CreateBackup;
            DeleteBackup = Texts.DeleteBackup;
            EditBackup = Texts.EditBackup;
            ListBackup = Texts.ListBackup;
            ExeBackup = Texts.ExeBackup;
            Settings = Texts.SettingsMenu;
            Exit = Texts.Exit;
            Welcome = Texts.HomeWelcome;
            ConfiguredBackupStr = Texts.HomeConfiguredBackup;
            HomeTipTitle = Texts.HomeTipTitle;
            HomeRandomTip = LoadRandomTip();

            var jobs = BackupAppService.GetAll();
            BackupNumber = jobs.Count();

            NavigateToSettingsCommand = new RelayCommand(() =>
            {
                NavigateTo(new SettingsMenuViewModel(mainWindow));
            });
            NavigateToListBackupCommand = new RelayCommand(() =>
            {
                NavigateTo(new ListBackupMenuViewModel(mainWindow));
            });
            //NavigateToCreateBackupCommand = new RelayCommand(() =>
            //{
            //    NavigateTo(new CreateBackupMenuViewModel(mainWindow, new DialogService()));
            //});
            NavigateToExecuteBackupCommand = new RelayCommand(() =>
            {
                NavigateTo(new ExecuteBackupMenuViewModel(mainWindow));
            });
            ExitCommand = new RelayCommand(OnExit);
        }

        private void OnExit()
        {
            if (Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.Shutdown();
            }
        }

        private string LoadRandomTip()
        {
            var tips = new List<string>
            {
                Texts.HomeTip1,
                Texts.HomeTip2,
                Texts.HomeTip3,
                Texts.HomeTip4,
                Texts.HomeTip5,
                Texts.HomeTip6,
                Texts.HomeTip7
            };

            var random = new Random();
            // Randomly chooses a tip
            string randomTip = tips[random.Next(tips.Count)];
            return randomTip;
        }
    }
}