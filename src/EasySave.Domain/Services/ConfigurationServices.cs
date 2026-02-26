using EasySave.Domain.Exceptions;
using EasySave.Domain.Interfaces;
using EasySave.Domain.Models;
using System.Text.Json;
using EasySave.Domain.Enums;

namespace EasySave.Domain.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly string _configFilePath;
        private readonly string _baseAppPath;

        private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

        public ConfigurationService()
        {
            // Resolve %APPDATA%\EasySave as the root for all persisted application data
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            _baseAppPath = Path.Combine(appDataPath, "EasySave");

            // Create the directory on first run so all subsequent writes can assume it exists
            if (!Directory.Exists(_baseAppPath))
                Directory.CreateDirectory(_baseAppPath);

            _configFilePath = Path.Combine(_baseAppPath, "config.json");
        }

        // Builds a safe default configuration so the app is usable without any manual setup.
        private ApplicationSettings GetDefaultSettings()
        {
            return new ApplicationSettings
            {
                Language = Language.English,
                LogDirectoryPath = Path.Combine(_baseAppPath, "Logs"),
                StateFileDirectoryPath = Path.Combine(_baseAppPath, "State"),
                LogFormat = 0,
                LogMode = 0,
                LogServerHost = "127.0.0.1",
                LogServerPort = 11000,
                BusinessSoftwareName = "CalculatorApp",
                CryptoSoftPath = Path.Combine(AppContext.BaseDirectory, "EasySave.CryptoSoft.exe"),
                CryptoSoftKeyPath = Path.Combine(_baseAppPath, "key.txt"),
                EncryptedFileExtensions = new List<string>(),
                PriorityFileExtensions = new List<string>(),
                MaxLargeFileSizeKb = 0
            };
        }

        public ApplicationSettings LoadSettings()
        {
            // No config file yet (first run) : return defaults without writing anything
            if (!File.Exists(_configFilePath))
                return GetDefaultSettings();

            try
            {
                string json = File.ReadAllText(_configFilePath);

                // Deserialise and fall back to defaults if the result is unexpectedly null
                var settings = JsonSerializer.Deserialize<ApplicationSettings>(json);
                return settings ?? GetDefaultSettings();
            }
            catch (JsonException ex)
            {
                // File exists but its content is malformed — surface a domain-level error
                throw new PersistenceException(EasySaveErrorCode.ConfigFileCorrupted, _configFilePath, ex);
            }
            catch (IOException ex)
            {
                // File exists but cannot be read (permissions, file lock, etc.)
                throw new PersistenceException(EasySaveErrorCode.ConfigFileUnreadable, _configFilePath, ex);
            }
        }

        public void EnsureConfigExists()
        {
            // Write defaults only on first run never overwrites a file the user has already edited
            if (!File.Exists(_configFilePath))
                SaveSettings(GetDefaultSettings());
        }

        public void EnsureKeyExists()
        {
            string keyPath = Path.Combine(_baseAppPath, "key.txt");

            // Generate a placeholder key on first run so CryptoSoft always has a key available
            if (!File.Exists(keyPath))
                File.WriteAllText(keyPath, "cryptosoft key");
        }

        public void SaveSettings(ApplicationSettings settings)
        {
            // Serialise and overwrite atomically from the caller's perspective
            string json = JsonSerializer.Serialize(settings, _jsonOptions);
            File.WriteAllText(_configFilePath, json);
        }

        public bool FileExists() => File.Exists(_configFilePath);
    }
}