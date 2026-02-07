using EasySave.Domain.Interfaces;
using EasySave.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EasySave.Domain.Services
{
    public class SettingsFileService : ISettingsFileService
    {
        private static readonly object _lock = new();
        private string _settingsFilePath;

        public void SetSettingsFilePath(string settingsDirectoryPath)
        {
            if (!Directory.Exists(settingsDirectoryPath))
                Directory.CreateDirectory(settingsDirectoryPath);

            _settingsFilePath = Path.Combine(settingsDirectoryPath, "settings.json");

            if (!File.Exists(_settingsFilePath))
                File.WriteAllText(_settingsFilePath, "{}");
        }

        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            Converters = { new JsonStringEnumConverter() }
        };

        public ApplicationSettings ReadSettings()
        {
            lock (_lock)
            {
                if (!File.Exists(_settingsFilePath)) return new ApplicationSettings();

                var json = File.ReadAllText(_settingsFilePath);
                return JsonSerializer.Deserialize<ApplicationSettings>(json, _jsonOptions)
                    ?? new ApplicationSettings();
            }
        }
           
        public void WriteSettings(ApplicationSettings settings)
        {
            lock (_lock)
            {
                var json = JsonSerializer.Serialize(settings, _jsonOptions);
                File.WriteAllText(_settingsFilePath, json);
            }
        }
    }
}