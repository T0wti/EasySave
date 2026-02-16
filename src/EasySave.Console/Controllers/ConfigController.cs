using EasySave.Application;
using EasySave.Application.Resources;
using EasySave.EasyLog.Enums;

namespace EasySave.Console.Controllers
{
    // Handles all configuration-related actions
    public class ConfigController
    {
        private readonly ConfigAppService _configAppService;
        private readonly ITextProvider _texts;

        public ConfigController(ConfigAppService configAppService, ITextProvider texts)
        {
            _configAppService = configAppService;
            _texts = texts;
        }

        public bool FileExists() => _configAppService.FileExists();

        public void EnsureConfigExists() => _configAppService.EnsureConfigExists();

        public void EnsureKeyExists() => _configAppService.EnsureKeyExists();

        public int GetLanguageCode() => _configAppService.Load().LanguageCode;

        public LogFormat GetLogFormat() => _configAppService.GetLogFormat();

        public void HandleChangeLanguage(int code)
        {
            try
            {
                _configAppService.ChangeLanguage(code);
            }
            catch 
            {
            }
        }

        public void HandleChangeLogFormat(int formatCode)
        {
            try
            {
                _configAppService.ChangeLogFormat(formatCode);
                System.Console.WriteLine(_texts.LogFormatChanged);
            }
            catch
            {
            }
        }
    }
}