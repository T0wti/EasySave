using EasySave.Domain.Enums;
using EasySave.Domain.Interfaces;
using EasySave.Domain.Models;
using System.Text.Json;
using System;
using System.IO;

public class ConfigurationService : IConfigurationService
{
    private static readonly Lazy<ConfigurationService> _instance = new(() => new ConfigurationService());
    public static ConfigurationService Instance => _instance.Value;

    private readonly string _configFilePath;
    private readonly string _baseAppPath;
    private ConfigurationService()
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
            LogDirectoryPath = Path.Combine(_baseAppPath, "Logs"),
            StateFileDirectoryPath = Path.Combine(_baseAppPath, "State")
        };
    }

    public ApplicationSettings LoadSettings()
    {
        if (!File.Exists(_configFilePath))
        {
            return GetDefaultSettings(); 
        }

        try
        {
            string json = File.ReadAllText(_configFilePath);
            var settings = JsonSerializer.Deserialize<ApplicationSettings>(json);
            return settings ?? GetDefaultSettings();
        }
        catch
        {
            return GetDefaultSettings();
        }
    }

    public void EnsureConfigExists()
    {
        if (!File.Exists(_configFilePath))
        {
            SaveSettings(GetDefaultSettings());
        }
    }

    public void SaveSettings(ApplicationSettings settings)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(settings, options);
        File.WriteAllText(_configFilePath, json);
    }

    public bool FileExists()
    {
        return File.Exists(_configFilePath);
    }
}