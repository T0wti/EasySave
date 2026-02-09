using EasySave.Domain.Interfaces;
using EasySave.Domain.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

public class FileStateService : IFileStateService
{
    // Singleton pattern: ensures only one instance of FileStateService exists
    private static readonly Lazy<FileStateService> _instance = new(() => new FileStateService());
    public static FileStateService Instance => _instance.Value;

    private string? _stateFilePath;
    private static readonly object _lock = new();

    // JSON serialization options
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() }
    };

    // Private constructor for singleton
    private FileStateService() { }

    // Initializes the service with the directory where the state file will be stored
    public void Initialize(string stateDirectoryPath)
    {
        lock (_lock)
        {
            if (_stateFilePath != null) return;

            if (!Directory.Exists(stateDirectoryPath))
                Directory.CreateDirectory(stateDirectoryPath);

            _stateFilePath = Path.Combine(stateDirectoryPath, "state.json");

            if (!File.Exists(_stateFilePath))
                File.WriteAllText(_stateFilePath, "[]");

        }
    }

    // Reads all backup progress states from the JSON file
    public List<BackupProgress> ReadState()
    {
        EnsureInitialized();

        lock (_lock)
        {
            var json = File.ReadAllText(_stateFilePath!);
            return JsonSerializer.Deserialize<List<BackupProgress>>(json, _jsonOptions) // Deserialize JSON into a list of BackupProgress objects; return empty list if null
                ?? new();
        }
    }

    // Writes the list of backup progress states to the JSON file
    public void WriteState(List<BackupProgress> states)
    {
        EnsureInitialized();

        lock (_lock)
        {
            var json = JsonSerializer.Serialize(states, _jsonOptions);
            File.WriteAllText(_stateFilePath!, json);
        }
    }

    // Helper method to ensure the service has been initialized
    private void EnsureInitialized()
    {
        if (_stateFilePath == null)
            throw new InvalidOperationException("FileStateService must be initialized before use.");
    }
}