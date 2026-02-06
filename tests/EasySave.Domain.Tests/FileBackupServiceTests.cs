using EasySave.Domain.Enums;
using EasySave.Domain.Models;
using EasySave.Domain.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Xunit;

namespace EasySave.Domain.Tests
{
    public class FileBackupServiceTests : IDisposable
    {
        private readonly string _tempDir;
        private readonly FileBackupService _service;

        public FileBackupServiceTests()
        {
            //Create temp directory
            _tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(_tempDir);

            _service = new FileBackupService();

            //To write in temp directory
            _service.SetFilePath(_tempDir);
        }

        public void Dispose()
        {
            //Cleanup after test
            if (Directory.Exists(_tempDir)) { Directory.Delete(_tempDir, recursive: true); }
        }

        [Fact]
        public void LoadJobs_WhenFileDoesNotExist_ReturnsEmptyList()
        {
            var filePath = Path.Combine(_tempDir, "jobs.json");
            //Make sure file does not already exists
            if (File.Exists(filePath)) { File.Delete(filePath); }

            var result = _service.LoadJobs();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void SaveJobs_ShouldCreateFileWithCorrectJson()
        {
            var jobs = new List<BackupJob>
            {
                new BackupJob(0, "JobTest1", "C:/Source", "D:/Source", BackupType.Full)
            };

            _service.SaveJobs(jobs);

            var filePath = Path.Combine(_tempDir, "jobs.json");
            Assert.True(File.Exists(filePath));

            string jsonContent = File.ReadAllText(filePath);
            //Checks job name
            Assert.Contains("JobTest1", jsonContent);
        }

        [Fact]
        public void LoadJobs_ShouldReturn_SavedJobs()
        {
            //Writing a JSON file
            var filePath = Path.Combine(_tempDir, "jobs.json");
            var json = @"
            [
                {
                    """"Name"""": """"JobFromJson"""",
                    """"SourcePath"""": """"Source"""",
                    """"TargetPath"""": """"Target"""",
                    """"Type"""": 1
                }
            ]";
            File.WriteAllText(filePath, json);

            var result = _service.LoadJobs();

            //Checks if list has 1 and only element
            Assert.Single(result);
            Assert.Equal("JobFromJson", result[0].Name);
        }
    }
}
