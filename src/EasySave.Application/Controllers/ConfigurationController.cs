using EasySave.Domain.Interfaces;
using EasySave.Domain.Models;
using EasySave.Domain.Enums;

namespace EasySave.Application.Controllers
{
    public class ConfigurationController
    {
        private readonly IConfigurationService _configService;

        public ConfigurationController(IConfigurationService configService)
        {
            _configService = configService;
        }

        public ApplicationSettings Load()
        {
            return _configService.LoadSettings();
        }

        public void ChangeLanguage(Language lang)
        {
            var settings = _configService.LoadSettings();
            settings.Language = lang;
            _configService.SaveSettings(settings);
        }
    }
}
