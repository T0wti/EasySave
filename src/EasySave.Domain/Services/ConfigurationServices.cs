using EasySave.Domain.Exceptions;
using EasySave.Domain.Interfaces;
using EasySave.Domain.Models;
using System;
using System.IO;
using System.Text.Json;
using EasySave.Domain.Enums;

namespace EasySave.Domain.Services
{
    // Service responsible for loading and saving config configurations
    public class ConfigurationService : IConfigurationService
    {   
        private readonly string _configFilePath;
        private readonly string _baseAppPath;
        private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };


        // Public constructor to allow DI
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
                Theme = ThemeMode.System,
                LogDirectoryPath = Path.Combine(_baseAppPath, "Logs"),
                StateFileDirectoryPath = Path.Combine(_baseAppPath, "State"),
                LogFormat = 0,
                BusinessSoftwareName = "CalculatorApp",
                CryptoSoftPath = Path.Combine(AppContext.BaseDirectory, "EasySave.CryptoSoft.exe"),
                CryptoSoftKeyPath = Path.Combine(_baseAppPath, "key.txt"),
                EncryptedFileExtensions = new List<string>()
            };
        }

        public ApplicationSettings LoadSettings()
        {
            if (!File.Exists(_configFilePath))
                return GetDefaultSettings();

            try
            {
                string json = File.ReadAllText(_configFilePath);
                var settings = JsonSerializer.Deserialize<ApplicationSettings>(json);
                return settings ?? GetDefaultSettings();
            }
            catch (JsonException ex)
            {
                throw new PersistenceException(
                    EasySaveErrorCode.ConfigFileCorrupted,
                    _configFilePath,
                    ex);
            }
            catch (IOException ex)
            {
                throw new PersistenceException(
                    EasySaveErrorCode.ConfigFileUnreadable,
                    _configFilePath,
                    ex);
            }
        }

        public void EnsureConfigExists()
        {
            if (!File.Exists(_configFilePath))
            {
                SaveSettings(GetDefaultSettings());
            }
        }

        public void EnsureKeyExists()
        {
            string keyPath = Path.Combine(_baseAppPath, "key.txt");

            if (!File.Exists(keyPath))
                File.WriteAllText(keyPath, "cryptosoft key");
        }

        public void SaveSettings(ApplicationSettings settings)
        {
            string json = JsonSerializer.Serialize(settings, _jsonOptions);
            File.WriteAllText(_configFilePath, json);
        }   

        public bool FileExists()
        {
            return File.Exists(_configFilePath);
        }
    }
}