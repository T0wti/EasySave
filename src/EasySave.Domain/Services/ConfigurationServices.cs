using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using EasySave.Domain.Interfaces;
using EasySave.Domain.Models;
using EasySave.Domain.Enums;

namespace EasySave.Domain.Services
{
    internal class ConfigurationService : IConfigurationService
    {
        private readonly string _configFilePath;

        public ConfigurationService()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string easySavePath = Path.Combine(appDataPath, "EasySave");

            if (!Directory.Exists(easySavePath))
            {
                Directory.CreateDirectory(easySavePath);
            }

            _configFilePath = Path.Combine(easySavePath, "config.json");
        }

        public ApplicationSettings LoadSettings()
        {
            if (!File.Exists(_configFilePath))
            {
                //If no file exists return default settings
                return new ApplicationSettings(Language.English);
            }

            try
            {
                string json = File.ReadAllText(_configFilePath);
                return JsonSerializer.Deserialize<ApplicationSettings>(json) ?? new ApplicationSettings(Language.English);
            }
            catch
            {
                return new ApplicationSettings(Language.English);
            }
        }

        public void SaveSettings(ApplicationSettings settings) { 
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(settings, options);
            File.WriteAllText(_configFilePath, json);
        }
    }
}
