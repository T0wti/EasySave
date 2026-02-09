using EasySave.Application.DTOs;
using EasySave.Domain.Enums;
using EasySave.Domain.Interfaces;

namespace EasySave.Application.Controllers
{
    public class ConfigurationController
    {
        private readonly IConfigurationService _configService;

        public ConfigurationController(IConfigurationService configService)
        {
            _configService = configService;
        }

        // Load configuration and map to DTO
        public ApplicationSettingsDto Load()
        {
            var settings = _configService.LoadSettings();

            return new ApplicationSettingsDto
            {
                LanguageCode = ConvertLanguageToCode(settings.Language)
            };
        }

        // Change language based on int code
        public void ChangeLanguage(int code)
        {
            var settings = _configService.LoadSettings();
            settings.Language = ConvertCodeToLanguage(code);
            _configService.SaveSettings(settings);
        }

        // Check if configuration file exists
        public bool FileExists()
        {
            return _configService.FileExists();
        }

        // Ensure configuration file exists
        public void EnsureConfigExists()
        {
            _configService.EnsureConfigExists();
        }

        // --- Private Methods ---

        private static int ConvertLanguageToCode(Language lang)
        {
            return lang == Language.French ? 0 : 1;
        }

        private static Language ConvertCodeToLanguage(int code)
        {
            return code == 0 ? Language.French : Language.English;
        }
    }
}
