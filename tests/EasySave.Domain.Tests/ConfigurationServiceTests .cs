using EasySave.Domain.Services;
using EasySave.Domain.Models;
using EasySave.Domain.Enums;
using EasySave.Domain.Exceptions;
using System.IO;
using Xunit;

namespace EasySave.Tests
{
    public class ConfigurationServiceTests : IDisposable
    {
        private readonly string _testDir;
        private readonly ConfigurationService _service;

        public ConfigurationServiceTests()
        {
            _testDir = Path.Combine(Path.GetTempPath(), "EasySaveTest");
            if (Directory.Exists(_testDir))
                Directory.Delete(_testDir, true);

            Directory.CreateDirectory(_testDir);

            // Create a subclass to inject the test path
            _service = new TestConfigurationService(_testDir);
        }

        public void Dispose()
        {
            if (Directory.Exists(_testDir))
                Directory.Delete(_testDir, true);
        }

        // Test that LoadSettings returns default settings if config file does not exist
        [Fact]
        public void LoadSettings_FileDoesNotExist_ReturnsDefaultSettings()
        {
            var settings = _service.LoadSettings();

            Assert.NotNull(settings);
            Assert.Equal(Language.English, settings.Language);
        }

        // Test that SaveSettings creates a config file
        [Fact]
        public void SaveSettings_CreatesConfigFile_FileExistsAfterSave()
        {
            var settings = _service.LoadSettings();
            _service.SaveSettings(settings);

            Assert.True(_service.FileExists());
        }

        // Test that EnsureConfigExists creates the config file if it is missing
        [Fact]
        public void EnsureConfigExists_CreatesFileIfMissing()
        {
            Assert.False(_service.FileExists());

            _service.EnsureConfigExists();

            Assert.True(_service.FileExists());
        }

        // Test that EnsureKeyExists creates the key.txt file if it does not exist
        [Fact]
        public void EnsureKeyExists_CreatesKeyFileIfMissing()
        {
            string keyPath = Path.Combine(_testDir, "key.txt");
            if (File.Exists(keyPath))
                File.Delete(keyPath);

            _service.EnsureKeyExists();

            Assert.True(File.Exists(keyPath));
        }

        // Test that LoadSettings throws PersistenceException when JSON is corrupted
        [Fact]
        public void LoadSettings_CorruptedJson_ThrowsPersistenceException()
        {
            File.WriteAllText(Path.Combine(_testDir, "config.json"), "{ invalid json }");

            var ex = Assert.Throws<PersistenceException>(() => _service.LoadSettings());

            Assert.Equal(EasySaveErrorCode.ConfigFileCorrupted, ex.ErrorCode);
        }

        // Subclass to override the config path for testing
        private class TestConfigurationService : ConfigurationService
        {
            public TestConfigurationService(string basePath)
            {
                // hack to override private _baseAppPath and _configFilePath
                var baseAppPathField = typeof(ConfigurationService)
                    .GetField("_baseAppPath", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                baseAppPathField!.SetValue(this, basePath);

                var configFilePathField = typeof(ConfigurationService)
                    .GetField("_configFilePath", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                configFilePathField!.SetValue(this, Path.Combine(basePath, "config.json"));
            }
        }
    }
}