using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasySave.Application.Resources;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EasySave.GUI.ViewModels
{

    public partial class SettingsMenuViewModel : ViewModelBase
    {
        // To get states for radio buttons
        // ObservableProperty automatically generates public property of the same name (PasclCase)
        [ObservableProperty] private bool _isLanguage1Selected;
        [ObservableProperty] private bool _isLanguage2Selected;

        [ObservableProperty] private bool _isLogFormat1Selected;
        [ObservableProperty] private bool _isLogFormat2Selected;
        [ObservableProperty] private bool _isLogDestination1Selected;
        [ObservableProperty] private bool _isLogDestination2Selected;
        [ObservableProperty] private bool _isLogDestination3Selected;
        [ObservableProperty] private string? _businessSoftware;
        [ObservableProperty] private string? _extensionsToEncrypt;
        [ObservableProperty] private string? _extensionsToPrioritize;
        [ObservableProperty] private decimal? _maxLargeFileSizeThreshold; // decimal type because NumericUpDown wants decimal

        // Commands 
        public ICommand ExitCommand { get; }
        public ICommand Language1Command { get; }
        public ICommand Language2Command { get; }
        public ICommand LogFormat1Command { get; }
        public ICommand LogFormat2Command { get; }
        public ICommand LogDestination1Command { get; }
        public ICommand LogDestination2Command { get; }
        public ICommand LogDestination3Command { get; }
        public ICommand SaveCommand { get; }

        // String to display
        public string Title { get; }
        public string Language1 { get; }
        public string Language2 { get; }
        public string LogFormat1 { get; }
        public string LogFormat2 { get; }
        public string LogDestination1 { get; }
        public string LogDestination2 { get; }
        public string LogDestination3 { get; }
        public string Business { get; }
        public string LanguageTitle { get; }
        public string LogFormatTitle { get; }
        public string LogDestinationTitle { get; }
        public string Exit { get; }
        public string Save { get; }
        public string MarkdownExe { get; }
        public string ExtensionsToEncryptTitle { get; }
        public string ExtensionsToPrioritizeTitle { get; }
        public string ExtensionWatermark { get; }
        public string MaxLargeFileSizeTitle { get; }
        public string MaxLargeFileSizeInfo { get; }

        public SettingsMenuViewModel(MainWindowViewModel mainWindow) : base(mainWindow)
        {
            Title = Texts.SettingsMenuTitle;
            Language1 = Texts.Language1;
            Language2 = Texts.Language2;
            LogFormat1 = Texts.LogFormat1;
            LogFormat2 = Texts.LogFormat2;
            LogDestination1 = Texts.LogDestination1;
            LogDestination2 = Texts.LogDestination2;
            LogDestination3 = Texts.LogDestination3;
            LanguageTitle = Texts.SettingsMenuLanguage;
            Business = Texts.SettingsMenuBusiness;
            LogFormatTitle = Texts.SettingsMenuLogFormat;
            LogDestinationTitle = Texts.LogDestinationMenuTitle;
            Exit = Texts.Exit;
            Save = Texts.Save;
            MarkdownExe = Texts.SettingsMenuExemple;
            BusinessSoftware = ConfigAppService.GetBusinessSoftwareName();
            ExtensionsToEncrypt = ConfigAppService.GetEncryptedExtensionText();
            ExtensionsToEncryptTitle = Texts.ExtensionToEncryptTitle;
            ExtensionsToPrioritize = ConfigAppService.GetPriorityExtensionText();
            ExtensionsToPrioritizeTitle = Texts.ExtensionToPrioritizeTitle;
            ExtensionWatermark = Texts.ExtensionWatermark;
            MaxLargeFileSizeTitle = Texts.MaxLargeFileSizeTitle;
            MaxLargeFileSizeInfo = Texts.MaxLargeFileSizeInfo;
            MaxLargeFileSizeThreshold = Convert.ToDecimal(ConfigAppService.GetMaxLargeFileSize());

            var currentSettings = ConfigAppService.Load();
            var currentFormat = ConfigAppService.GetLogFormat();
            var currentDestination = ConfigAppService.GetLogMode();

            // Check which language is selected
            if (currentSettings.LanguageCode == 0) IsLanguage1Selected = true;
            else IsLanguage2Selected = true;

            // Check which log format is selected
            if (currentFormat == 0) IsLogFormat1Selected = true;
            else IsLogFormat2Selected = true;

            // Check which log destination is selected
            if (currentDestination == 0) IsLogDestination1Selected = true;
            else if (currentDestination == 1) IsLogDestination2Selected = true;
            else IsLogDestination3Selected = true;

            // Handle the language change
            Language1Command = new RelayCommand(() =>
            {
                ChangeLanguage(new FrenchTextProvider());
            });
            Language2Command = new RelayCommand(() =>
            {
                ChangeLanguage(new EnglishTextProvider());
            });

            // Handle the log format change
            LogFormat1Command = new RelayCommand(() =>
            {
                ChangeLogFormat(0);
            });
            LogFormat2Command = new RelayCommand(() =>
            {
                ChangeLogFormat(1);
            });

            // Handle the log destination change
            LogDestination1Command = new RelayCommand(() =>
            {
                ChangeLogDestination(0);
            });
            LogDestination2Command = new RelayCommand(() =>
            {
                ChangeLogDestination(1);
            });
            LogDestination3Command = new RelayCommand(() =>
            {
                ChangeLogDestination(2);
            });

            // Save button
            SaveCommand = new AsyncRelayCommand(SaveChange);

            ExitCommand = new RelayCommand(NavigateToBase);
        }

        private async Task SaveChange()
        {
            ConfigAppService.SaveBusinessSoftwareName(BusinessSoftware);
            ConfigAppService.SaveEncryptedExtensionText(ExtensionsToEncrypt);
            ConfigAppService.SavePriorityExtensionText(ExtensionsToPrioritize);

            // To prevent crash if null
            if(MaxLargeFileSizeThreshold != null) ConfigAppService.SaveMaxLargeFileSize((long)MaxLargeFileSizeThreshold);

            await ShowMessageAsync(Texts.MessageBoxSettingsSaved,"", "", Texts.MessageBoxOk, false, false);
        }

        // To prevent error
        private void OnMaxLargeFileSizeThresholdChanged(decimal value)
        {
            if (value == null) value = 0;
        }
    }
}