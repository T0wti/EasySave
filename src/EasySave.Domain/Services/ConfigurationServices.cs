using EasySave.Domain.Enums;
using EasySave.Domain.Interfaces;
using EasySave.Domain.Models;
using System.Text.Json;
using System;
using System.IO;


namespace EasySave.Domain.Services
{
    // Service responsible for loading and saving config configurations
    public class ConfigurationService : IConfigurationService
    {
        // Singleton pattern: ensures only one instance exists
        private static readonly Lazy<ConfigurationService> _instance = new(() => new ConfigurationService());
        public static ConfigurationService Instance => _instance.Value;

        private readonly string _configFilePath;
        private readonly string _baseAppPath;

        // Private constructor to enforce singleton pattern
        private ConfigurationService()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            _baseAppPath = Path.Combine(appDataPath, "EasySave");

            if (!Directory.Exists(_baseAppPath))
                Directory.CreateDirectory(_baseAppPath);

            _configFilePath = Path.Combine(_baseAppPath, "config.json");
        }

        // Returns the default application settings
        private ApplicationSettings GetDefaultSettings()
        {
            return new ApplicationSettings
            {
                MaxBackupJobs = 5,
                LogDirectoryPath = Path.Combine(_baseAppPath, "Logs"),
                StateFileDirectoryPath = Path.Combine(_baseAppPath, "State"),
                LogFormat = 0
            };
        }

        // Loads settings from the configuration file
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

        // Ensures that a configuration file exists
        public void EnsureConfigExists()
        {
            if (!File.Exists(_configFilePath))
            {
                SaveSettings(GetDefaultSettings());
            }
        }

        // Saves application settings to the configuration file
        public void SaveSettings(ApplicationSettings settings)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(settings, options);
            File.WriteAllText(_configFilePath, json);
        }

        // Checks if the configuration file exists
        public bool FileExists()
        {
            return File.Exists(_configFilePath);
        }
    }
}