using EasySave.Domain.Enums;
using EasySave.Domain.Exceptions;
using EasySave.Domain.Interfaces;
using EasySave.Domain.Models;
using System.Text.Json;

namespace EasySave.Domain.Services
{
    // Service responsible for loading and saving backup job configurations
    public class FileBackupService : IFileBackupService
    {
        private string _jobsFilePath;
        private readonly string _baseAppPath;
        private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

        // Public constructor for DI
        public FileBackupService()
        {
            // Anchor all persisted data to %APPDATA%\EasySave, consistent with ConfigurationService
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            _baseAppPath = Path.Combine(appDataPath, "EasySave");

            _jobsFilePath = Path.Combine(_baseAppPath, "jobs.json");
        }

        public List<BackupJob> LoadJobs()
        {
            // No file yet (first run) : return an empty list rather than throwing
            if (!File.Exists(_jobsFilePath))
                return new List<BackupJob>();

            try
            {
                string json = File.ReadAllText(_jobsFilePath);

                // Treat an empty or whitespace only file as "no jobs" rather than a parse error
                if (string.IsNullOrWhiteSpace(json))
                    return new List<BackupJob>();

                return JsonSerializer.Deserialize<List<BackupJob>>(json) ?? new List<BackupJob>();
            }
            catch (JsonException ex)
            {
                throw new PersistenceException(
                    EasySaveErrorCode.JobsFileCorrupted,
                    _jobsFilePath,
                    ex);
            }
            catch (IOException ex)
            {
                throw new PersistenceException(
                    EasySaveErrorCode.JobsFileUnreadable,
                    _jobsFilePath,
                    ex);
            }
        }

        public void SaveJobs(List<BackupJob> jobs)
        {
            // Write the job in the file
            string json = JsonSerializer.Serialize(jobs, _jsonOptions);
            File.WriteAllText(_jobsFilePath, json);
        }

        // For unit tests
        public void SetFilePath(string directoryPath)
        {
            _jobsFilePath = Path.Combine(directoryPath, "jobs.json");
        }
    }
}