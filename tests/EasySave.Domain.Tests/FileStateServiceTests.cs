using EasySave.Domain.Models;
using EasySave.Domain.Services;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace EasySave.Tests.Domain.Services
{
    public class FileStateServiceTests : IDisposable
    {
        private readonly string _testDirectory;

        public FileStateServiceTests()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        }

        public void Dispose()
        {
            if (Directory.Exists(_testDirectory))
                Directory.Delete(_testDirectory, true);
        }

        // Checks that the constructor creates the directory if it does not exist
        [Fact]
        public void Constructor_ShouldCreateDirectory_IfNotExists()
        {
            var service = new FileStateService(_testDirectory);
            Assert.True(Directory.Exists(_testDirectory));
        }

        // Checks that the constructor creates the state.json file if it does not exist
        [Fact]
        public void Constructor_ShouldCreateStateFile_IfNotExists()
        {
            var service = new FileStateService(_testDirectory);
            var stateFilePath = Path.Combine(_testDirectory, "state.json");
            Assert.True(File.Exists(stateFilePath));
            Assert.Equal("[]", File.ReadAllText(stateFilePath));
        }

        // Checks that ReadState returns an empty list when the file contains an empty array
        [Fact]
        public void ReadState_ShouldReturnEmptyList_WhenFileIsEmptyArray()
        {
            var service = new FileStateService(_testDirectory);
            var result = service.ReadState();
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        // Checks that the constructor throws ArgumentException for invalid path arguments
        [Fact]
        public void Constructor_ShouldThrowArgumentException_WhenPathIsInvalid()
        {
            Assert.Throws<ArgumentException>(() => new FileStateService(null));
            Assert.Throws<ArgumentException>(() => new FileStateService(""));
            Assert.Throws<ArgumentException>(() => new FileStateService(" "));
        }
    }
}