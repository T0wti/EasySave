using EasySave.Domain.Enums;
using EasySave.Domain.Models;
using EasySave.Domain.Services;
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit;

namespace EasySave.Domain.Tests
{
    public class FileStateServiceTest : IDisposable
    {
        private readonly string _tempDir;
        private readonly FileStateService _service;

        // FileStateService creates a unique temporary directory for each test and automatically cleans it afterward.
        public FileStateServiceTest()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            _service = new FileStateService();
        }

        public void Dispose()
        {
            if (Directory.Exists(_tempDir))
            {
                Directory.Delete(_tempDir, true);
            }
        }

        [Fact]
        // SetStateFilePath_CreatesDirectoryAndFile tests that the directory and file are created if they do not exist
        public void SetStateFilePath_CreatesDirectoryAndFile()
        {
            _service.SetStateFilePath(_tempDir);
            var stateFilePath = Path.Combine(_tempDir, "state.json");

            Assert.True(Directory.Exists(_tempDir));
            Assert.True(File.Exists(stateFilePath));
            Assert.Equal("[]", File.ReadAllText(stateFilePath));
        }

        [Fact]
        // SetStateFilePath_DoesNotOverwriteExistingFile tests that an existing file is not overwritten
        public void SetStateFilePath_DoesNotOverwriteExistingFile()
        {
            //var service = new FileStateService();
            Directory.CreateDirectory(_tempDir);
            var stateFilePath = Path.Combine(_tempDir, "state.json");
            File.WriteAllText(stateFilePath, "Test for overwrite existing file");

            _service.SetStateFilePath(_tempDir);

            var content = File.ReadAllText(stateFilePath);
            Assert.Equal("Test for overwrite existing file", content);
        }

        [Fact]
        // ReadState_WhenFileDoesNotExist checks empty list result when the file does not exists
        public void ReadState_WhenFileDoesNotExist()
        {
            _service.SetStateFilePath(_tempDir);
            var stateFilePath = Path.Combine(_tempDir, "state.json");
            File.Delete(stateFilePath);

            var result = _service.ReadState();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        /*
         * ReadState_ReturnsObjectsWhenJsonIsValid :
         * init a json and 
         * checks converts in BackupProgress
         * reads it
        */
        public void ReadState_ReturnsObjectsWhenJsonIsValid()
        {
            _service.SetStateFilePath(_tempDir);
            var stateFilePath = Path.Combine(_tempDir, "state.json");

            var json = """
            [
                {
                    "BackupName": "TestBackup",
                    "BackupJobId": 1,
                    "State": "Active"
                }
            ]
            """;

            File.WriteAllText(stateFilePath, json);

            var result = _service.ReadState();

            Assert.Single(result);
            Assert.Equal("TestBackup", result[0].BackupName);
            Assert.Equal(BackupJobState.Active, result[0].State);
        }

        [Fact]
        /*
         * WriteState_ShouldCreatesFileWithCorrectJson :
         * init a BackupProgress object
         * checks json file creation and good application
         */
        public void WriteState_ShouldCreatesFileWithCorrectJson()
        {
            _service.SetStateFilePath(_tempDir);
            var stateFilePath = Path.Combine(_tempDir, "state.json");

            var states = new List<BackupProgress>
            {
                new BackupProgress
                {
                    BackupName = "TestBackup",
                    BackupJobId = 1,
                    State = BackupJobState.Inactive,
                }
            };

            _service.WriteState(states);

            Assert.True(File.Exists(stateFilePath));

            var jsonContent = File.ReadAllText(stateFilePath);
            var deserialized = JsonSerializer.Deserialize<List<BackupProgress>>(jsonContent, new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() }
            });

            Assert.NotNull(deserialized);
            Assert.Equal("TestBackup", deserialized[0].BackupName);
            Assert.Equal(BackupJobState.Inactive, deserialized[0].State);
        }
    }
}