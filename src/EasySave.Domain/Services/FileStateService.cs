using EasySave.Domain.Interfaces;
using EasySave.Domain.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EasySave.Domain.Services
{
    // Persists the runtime state of all backup jobs to a single JSON file
    public class FileStateService : IFileStateService
    {
        private readonly string _stateFilePath;
        private readonly object _lock = new();


        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, // Omit null fields to keep the state file clean and readable
            Converters = { new JsonStringEnumConverter() }
        };

        public FileStateService(string stateDirectoryPath)
        {
            if (string.IsNullOrWhiteSpace(stateDirectoryPath))
                throw new ArgumentException("State directory path cannot be null or empty.", nameof(stateDirectoryPath));

            // Create the directory on first run so subsequent file operations never fail on a missing path
            if (!Directory.Exists(stateDirectoryPath))
                Directory.CreateDirectory(stateDirectoryPath);

            _stateFilePath = Path.Combine(stateDirectoryPath, "state.json");

            // Initialise with an empty JSON array so ReadState never encounters a missing or empty file
            if (!File.Exists(_stateFilePath))
                File.WriteAllText(_stateFilePath, "[]");
        }

        public List<BackupProgress> ReadState()
        {
            lock (_lock)
            {
                var json = File.ReadAllText(_stateFilePath);

                // Return the actual state
                return JsonSerializer.Deserialize<List<BackupProgress>>(json, _jsonOptions) ?? new();
            }
        }

        public void WriteState(List<BackupProgress> states)
        {
            lock (_lock)
            {
                var json = JsonSerializer.Serialize(states, _jsonOptions);

                // Write file on every call
                File.WriteAllText(_stateFilePath, json);
            }
        }
    }
}