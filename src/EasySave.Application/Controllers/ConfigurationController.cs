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

    public ApplicationSettingsDto Load()
    {
        var settings = _configService.LoadSettings();

        // Map Domain enum → int
        int code = settings.Language == Language.French ? 0 : 1;

        return new ApplicationSettingsDto
        {
            LanguageCode = code
        };
    }

    public void ChangeLanguage(int code)
    {
        var settings = _configService.LoadSettings();

        // Map int → Domain enum
        settings.Language = code == 0 ? Language.French : Language.English;

        _configService.SaveSettings(settings);
    }

    public bool FileExists()
    {
        var _exist = _configService.FileExists();
        return _exist;
    }

    public void EnsureConfigExists()
    {
        _configService.EnsureConfigExists();
    }
}
