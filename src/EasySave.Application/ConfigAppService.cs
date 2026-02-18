using EasySave.Application.DTOs;
using EasySave.Application.Exceptions;
using EasySave.Domain.Enums;
using EasySave.Domain.Interfaces;
using EasySave.EasyLog;
using EasySave.EasyLog.Enums;

namespace EasySave.Application
{
    public class ConfigAppService
    {
        private readonly IConfigurationService _configService;

        public ConfigAppService(IConfigurationService configService)
        {
            _configService = configService;
        }

        // Load configuration and map to DTO
        public ApplicationSettingsDto Load()
        {
            try
            {
                var settings = _configService.LoadSettings();

                return new ApplicationSettingsDto
                {
                    LanguageCode = ConvertLanguageToCode(settings.Language)
                };
            }
            catch (EasySaveException ex) { throw DomainExceptionMapper.Map(ex); }
        }

        // Change language based on int code
        public void ChangeLanguage(int code)
        {
            try
            {
                var settings = _configService.LoadSettings();
                settings.Language = ConvertCodeToLanguage(code);
                _configService.SaveSettings(settings);
            }
            catch (EasySaveException ex) { throw DomainExceptionMapper.Map(ex); }

        }

        // 
        public void ChangeLogFormat(int code)
        {
            try
            {
                var format = ConvertCodeToLogFormat(code);
                var settings = _configService.LoadSettings();

                EasyLogService.Instance.Reset();
                EasyLogService.Instance.Initialize(
                    settings.LogDirectoryPath,
                    format
                );

                settings.LogFormat = (int)format;
                _configService.SaveSettings(settings);
            }
            catch (EasySaveException ex) { throw DomainExceptionMapper.Map(ex); }

        }

        public LogFormat GetLogFormat()
        {
            var settings = _configService.LoadSettings();
            return ConvertCodeToLogFormat(Convert.ToInt32(settings.LogFormat.ToString()));
        }

        // Check if configuration file exists
        public bool FileExists()
        {
            return _configService.FileExists();
        }

        // Ensure configuration file exists
        public void EnsureConfigExists()
        {
            _configService. EnsureConfigExists();
        }

        public void EnsureKeyExists() //Temporaire
        {
            _configService.EnsureKeyExists();
        }

        // Return the current extensions
        private List<string> GetEncryptedExtensions()
        {
            var settings = _configService.LoadSettings();
            return settings.EncryptedFileExtensions ?? new List<string>();
        }

        public string GetEncryptedExtensionText()
        {
            var extensions = GetEncryptedExtensions();
            var str = string.Join(", ", extensions);
            str = str.Replace(".", "");
            if (str==".")
            {
                return string.Empty;
            }
            else
            {
                return str;
            }
        }
        
        // Update the extensions list
        private void SaveEncryptedExtensions(List<string> extensions)
        {
            var settings = _configService.LoadSettings();
            settings.EncryptedFileExtensions = extensions
                .Select(e => e.StartsWith(".") ? e.ToLower() : $".{e.ToLower()}")
                .Distinct()
                .ToList();
            _configService.SaveSettings(settings);
        }

        public void SaveEncryptedExtensionText(string? text)
        {
            if (text != null)
            {
                text = text.Replace(" ", "");
                var extensions = text.Split(',').ToList();
                SaveEncryptedExtensions(extensions);
            }
            else
            {
                SaveEncryptedExtensions(null);
            }
        }

        // Return business software business
        public string? GetBusinessSoftwareName()
        {
            var settings = _configService.LoadSettings();
            return settings.BusinessSoftwareName;
        }

        // Update the business software
        public void SaveBusinessSoftwareName(string? name)
        {
            var settings = _configService.LoadSettings();
            settings.BusinessSoftwareName = string.IsNullOrWhiteSpace(name) ? null : name.Trim();
            _configService.SaveSettings(settings);
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

        private static LogFormat ConvertCodeToLogFormat(int code)
        {
            return code == 0 ? LogFormat.Json : LogFormat.Xml;
        }
    }
}
