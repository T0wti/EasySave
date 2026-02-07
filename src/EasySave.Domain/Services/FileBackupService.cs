using EasySave.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using EasySave.Domain.Interfaces;
using EasySave.Domain.Models;

namespace EasySave.Domain.Services
{
    public class FileBackupService : IFileBackupService
    {
        //Singleton
        private static readonly Lazy<FileBackupService> _instance = new(() => new FileBackupService());
        public static FileBackupService Instance => _instance.Value;

        private string _jobsFilePath;

        private FileBackupService()
        {
            _jobsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jobs.json");
        }

        public List<BackupJob> LoadJobs()
        {
            if (!File.Exists(_jobsFilePath)) return new List<BackupJob>();

            try
            {
                string json = File.ReadAllText(_jobsFilePath);
                if (string.IsNullOrWhiteSpace(json)) return new List<BackupJob>();
                return JsonSerializer.Deserialize<List<BackupJob>>(json) ?? new List<BackupJob>();
            }
            catch
            {
                return new List<BackupJob>();
            }
        }

        public void SaveJobs(List<BackupJob> jobs)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(jobs, options);
            File.WriteAllText(_jobsFilePath, json);
        }

        //For unit tests
        public void SetFilePath(string directoryPath)
        {
            _jobsFilePath = Path.Combine(directoryPath, "jobs.json");
        }
    }
}
