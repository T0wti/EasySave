using EasySave.Domain.Interfaces;
using EasySave.Domain.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

public class FileStateService : IFileStateService
{
    //Gestion du singleton
    private static readonly Lazy<FileStateService> _instance = new(() => new FileStateService());
    public static FileStateService Instance => _instance.Value;

    private string? _stateFilePath;
    private static readonly object _lock = new();

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() }
    };

    private FileStateService() { }

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

    public List<BackupProgress> ReadState()
    {
        EnsureInitialized();

        lock (_lock)
        {
            var json = File.ReadAllText(_stateFilePath!);
            return JsonSerializer.Deserialize<List<BackupProgress>>(json, _jsonOptions) 
                ?? new();
        }
    }

    public void WriteState(List<BackupProgress> states)
    {
        EnsureInitialized();

        lock (_lock)
        {
            var json = JsonSerializer.Serialize(states, _jsonOptions);
            File.WriteAllText(_stateFilePath!, json);
        }
    }
    private void EnsureInitialized()
    {
        if (_stateFilePath == null)
            throw new InvalidOperationException("FileStateService must be initialized before use.");
    }
}