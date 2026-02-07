using EasySave.Domain.Enums;
using EasySave.Domain.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasySave.Domain.Tests
{
    public class SettingsFileServiceTest
    {
        private readonly string _tempDir;
        private readonly SettingsFileService _settings;
        public SettingsFileServiceTest()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            _settings = new SettingsFileService();
        }
        [Fact]
        /*
         * ReadSettings_ReturnsObjectsWhenJsonIsValid :
         * init a json  
         * checks converts in BackupProgress
         * reads it
        */
        public void ReadSettings_ReturnsObjectsWhenJsonIsValid()
        {
            _settings.SetSettingsFilePath(_tempDir);
            var stateFilePath = Path.Combine(_tempDir, "settings.json");

            var json = """
            {
                "Language": "English",
                "MaxBackupJobs": 5,
                "LogDirectoryPath": "C:\\Logs",
                "StateFileDirectoryPath": "C:\\State"
            }
            """;

            File.WriteAllText(stateFilePath, json);

            var result = _settings.ReadSettings();

            Assert.Equal(Language.English, result.Language);
            Assert.Equal(5, result.MaxBackupJobs);
            Assert.Equal("C:\\Logs", result.LogDirectoryPath);
            Assert.Equal("C:\\State", result.StateFileDirectoryPath);
        }
    }
}
