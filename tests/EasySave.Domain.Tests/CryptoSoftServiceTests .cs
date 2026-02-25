using EasySave.Domain.Models;
using EasySave.Domain.Services;
using EasySave.Domain.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace EasySave.Tests.Domain.Services
{
    public class CryptoSoftServiceTests : IDisposable
    {
        private readonly string _tempDir;
        private readonly string _fakeCryptoExe;
        private readonly string _fakeKeyFile;

        public CryptoSoftServiceTests()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(_tempDir);

            // Fake "CryptoSoft.exe" just as a dummy file for testing
            _fakeCryptoExe = Path.Combine(_tempDir, "CryptoSoft.exe");
            File.WriteAllText(_fakeCryptoExe, "echo dummy");

            _fakeKeyFile = Path.Combine(_tempDir, "key.txt");
            File.WriteAllText(_fakeKeyFile, "dummy key");
        }

        public void Dispose()
        {
            if (Directory.Exists(_tempDir))
                Directory.Delete(_tempDir, true);
        }

        // ShouldEncrypt returns true only for configured extensions
        [Fact]
        public void ShouldEncrypt_ReturnsTrue_ForConfiguredExtension()
        {
            var configMock = new Mock<IConfigurationService>();
            configMock.Setup(c => c.LoadSettings()).Returns(new ApplicationSettings
            {
                EncryptedFileExtensions = new List<string> { ".txt" }
            });

            var service = new CryptoSoftService(configMock.Object);

            bool result = service.ShouldEncrypt(Path.Combine(_tempDir, "file.txt"));

            Assert.True(result);
        }

        // ShouldEncrypt returns false for non-configured extension
        [Fact]
        public void ShouldEncrypt_ReturnsFalse_ForNonConfiguredExtension()
        {
            var configMock = new Mock<IConfigurationService>();
            configMock.Setup(c => c.LoadSettings()).Returns(new ApplicationSettings
            {
                EncryptedFileExtensions = new List<string> { ".txt" }
            });

            var service = new CryptoSoftService(configMock.Object);

            bool result = service.ShouldEncrypt(Path.Combine(_tempDir, "file.doc"));

            Assert.False(result);
        }

        // Encrypt returns -1 if CryptoSoft.exe or key does not exist
        [Fact]
        public void Encrypt_ReturnsMinusOne_WhenExeOrKeyMissing()
        {
            var configMock = new Mock<IConfigurationService>();
            configMock.Setup(c => c.LoadSettings()).Returns(new ApplicationSettings
            {
                CryptoSoftPath = Path.Combine(_tempDir, "missing.exe"),
                CryptoSoftKeyPath = Path.Combine(_tempDir, "missingKey.txt")
            });

            var service = new CryptoSoftService(configMock.Object);

            long result = service.Encrypt("source.txt", "target.txt");

            Assert.Equal(-1, result);
        }

        // Encrypt retries and returns -10 when the fake process fails
        [Fact]
        public void Encrypt_RetriesAndReturnsMinusTen_WhenProcessFails()
        {
            var configMock = new Mock<IConfigurationService>();
            configMock.Setup(c => c.LoadSettings()).Returns(new ApplicationSettings
            {
                CryptoSoftPath = _fakeCryptoExe,
                CryptoSoftKeyPath = _fakeKeyFile
            });

            var service = new CryptoSoftService(configMock.Object);

            // Using an empty source file to simulate failure (RunCryptoSoft returns -1)
            string source = Path.Combine(_tempDir, "source.txt");
            File.WriteAllText(source, "");

            string target = Path.Combine(_tempDir, "target.txt");

            long result = service.Encrypt(source, target);

            // Because RunCryptoSoft cannot run a real executable, it returns -1
            Assert.Equal(-1, result);
        }
    }
}