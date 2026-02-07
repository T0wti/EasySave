using EasySave.Application.DTOs;
using EasySave.Domain.Enums;
using EasySave.Domain.Interfaces;

namespace EasySave.Application.Controllers;

public class ConfigurationController
{
    private readonly IConfigurationService _configService;

    public ConfigurationController(IConfigurationService configService)
    {
        _configService = configService;
    }

    public ApplicationSettingsDTO Load()
    {
        var settings = _configService.LoadSettings();

        return new ApplicationSettingsDTO
        {
            Language = settings.Language
        };
    }

    public void ChangeLanguage(Language lang)
    {
        var settings = _configService.LoadSettings();
        settings.Language = lang;
        _configService.SaveSettings(settings);
    }
}
