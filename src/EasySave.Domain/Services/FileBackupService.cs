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
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            _baseAppPath = Path.Combine(appDataPath, "EasySave");

            _jobsFilePath = Path.Combine(_baseAppPath, "jobs.json");
        }

        public List<BackupJob> LoadJobs()
        {
            if (!File.Exists(_jobsFilePath))
                return new List<BackupJob>();

            try
            {
                string json = File.ReadAllText(_jobsFilePath);
                if (string.IsNullOrWhiteSpace(json))
                    return new List<BackupJob>();

                return JsonSerializer.Deserialize<List<BackupJob>>(json) ?? new List<BackupJob>();
            }
            catch (JsonException ex)
            {
                throw new PersistenceException(
                    "Failed to load backup jobs: the jobs file is corrupted.",
                    _jobsFilePath,
                    ex);

            }
            catch (IOException ex)
            {
                throw new PersistenceException(
                    "Failed to load backup jobs: unable to read the jobs file.",
                    _jobsFilePath,
                    ex);
            }
        }

        public void SaveJobs(List<BackupJob> jobs)
        {
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