using EasySave.Domain.Enums;
using EasySave.Domain.Interfaces;
using EasySave.Domain.Models;
using System.Text.Json;

public class ConfigurationService : IConfigurationService
{
    private readonly string _configFilePath;
    private readonly string _baseAppPath;

    public ConfigurationService()
    {
        string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        _baseAppPath = Path.Combine(appDataPath, "EasySave");

        if (!Directory.Exists(_baseAppPath))
            Directory.CreateDirectory(_baseAppPath);

        _configFilePath = Path.Combine(_baseAppPath, "config.json");
    }

    private ApplicationSettings GetDefaultSettings()
    {
        return new ApplicationSettings
        {
            Language = Language.English,
            MaxBackupJobs = 5,
            // On centralise la création des sous-dossiers ici
            LogDirectoryPath = Path.Combine(_baseAppPath, "Logs"),
            StateFileDirectoryPath = Path.Combine(_baseAppPath, "State")
        };
    }

    public ApplicationSettings LoadSettings()
    {
        if (!File.Exists(_configFilePath))
        {
            var defaultSettings = GetDefaultSettings();
            SaveSettings(defaultSettings); // On l'écrit sur le disque immédiatement
            return defaultSettings;
        }

        try
        {
            string json = File.ReadAllText(_configFilePath);
            var settings = JsonSerializer.Deserialize<ApplicationSettings>(json);

            // Sécurité : si le JSON est corrompu ou vide, on rend le défaut
            return settings ?? GetDefaultSettings();
        }
        catch
        {
            return GetDefaultSettings();
        }
    }

    public void SaveSettings(ApplicationSettings settings)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(settings, options);
        File.WriteAllText(_configFilePath, json);
    }
}