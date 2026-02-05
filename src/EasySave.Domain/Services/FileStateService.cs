using EasySave.Domain.Interfaces;
using EasySave.Domain.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EasySave.Domain.Services
{
    public class FileStateService : IFileStateService
    {
        private static readonly object _lock = new();
        private string _stateFilePath;

        public void SetStateFilePath(string stateDirectoryPath)
        {
            if (!Directory.Exists(stateDirectoryPath))
                Directory.CreateDirectory(stateDirectoryPath);

            _stateFilePath = Path.Combine(stateDirectoryPath, "state.json");

            if (!File.Exists(_stateFilePath))
            {
                File.WriteAllText(_stateFilePath, "[]");
            }
        }

        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new JsonStringEnumConverter() } // Permet de lire et écrire les enums en texte
        };

        public List<BackupProgress> ReadState()
        {
            lock (_lock)
            {
                if (!File.Exists(_stateFilePath)) return new List<BackupProgress>();

                var json = File.ReadAllText(_stateFilePath);
                return JsonSerializer.Deserialize<List<BackupProgress>>(json, _jsonOptions)
                       ?? new List<BackupProgress>();
            }
        }

        public void WriteState(List<BackupProgress> states)
        {
            lock (_lock)
            {
                var json = JsonSerializer.Serialize(states, _jsonOptions);
                File.WriteAllText(_stateFilePath, json);
            }
        }

    }
}

