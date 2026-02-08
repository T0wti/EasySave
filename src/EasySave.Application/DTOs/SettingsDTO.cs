namespace EasySave.Application.DTOs;


public class ApplicationSettingsDto
{
    /// <summary>
    /// 0 = Français, 1 = Anglais
    /// </summary>
    public int LanguageCode { get; set; } = 1; // 1 = Anglais par défaut
}