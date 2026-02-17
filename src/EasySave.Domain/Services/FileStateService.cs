using EasySave.Domain.Interfaces;
using EasySave.Domain.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EasySave.Domain.Services
{
    public class FileStateService : IFileStateService
    {
        private readonly string _stateFilePath;
        private readonly object _lock = new();


        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new JsonStringEnumConverter() }
        };

        public FileStateService(string stateDirectoryPath)
        {
            if (string.IsNullOrWhiteSpace(stateDirectoryPath))
                throw new ArgumentException("State directory path cannot be null or empty.", nameof(stateDirectoryPath));

            if (!Directory.Exists(stateDirectoryPath))
                Directory.CreateDirectory(stateDirectoryPath);

            _stateFilePath = Path.Combine(stateDirectoryPath, "state.json");

            // Création automatique du fichier si absent
            if (!File.Exists(_stateFilePath))
                File.WriteAllText(_stateFilePath, "[]");
        }

        public List<BackupProgress> ReadState()
        {
            lock (_lock)
            {
                var json = File.ReadAllText(_stateFilePath);
                return JsonSerializer.Deserialize<List<BackupProgress>>(json, _jsonOptions) ?? new();
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