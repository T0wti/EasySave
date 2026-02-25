using EasySave.Domain.Interfaces;
using EasySave.Domain.Models;
using EasySave.Domain.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xunit;

namespace EasySave.Domain.Tests
{
    public class DifferentialBackupStrategyTest
    {
        // This method instantiates a DifferentialBackupStrategy with mocked services
        private DifferentialBackupStrategy CreateService(List<FileDescriptor> sourceFiles, List<FileDescriptor> targetFiles)
        {
            var fileServiceMock = new Mock<IFileService>();
            fileServiceMock.Setup(f => f.GetFiles("src")).Returns(sourceFiles);
            fileServiceMock.Setup(f => f.GetFiles("dest")).Returns(targetFiles);

            return new DifferentialBackupStrategy(fileServiceMock.Object);
        }

        [Fact]
        // Checks the copy for a new file
        public void GetFilesToCopy_ShouldCopyNewFiles()
        {
            var sourceFiles = new List<FileDescriptor>
            {
                new FileDescriptor { FullPath="src/a.txt", Size=10 },
                new FileDescriptor { FullPath="src/b.txt", Size=20 }
            };

            var targetFiles = new List<FileDescriptor>();

            var service = CreateService(sourceFiles, targetFiles);

            var toCopy = service.GetFilesToCopy("src", "dest");

            Assert.Equal(2, toCopy.Count);
            Assert.Contains(toCopy, f => f.FullPath == "src/a.txt");
            Assert.Contains(toCopy, f => f.FullPath == "src/b.txt");
        }

        [Fact]
        // Checks the copy when the file size or hash differs
        public void GetFilesToCopy_ShouldCopyFilesWithDifferentSizeAndHash()
        {
            var sourceFiles = new List<FileDescriptor>
            {
                new FileDescriptor { FullPath="src/a.txt", Size=10 },
                new FileDescriptor { FullPath="src/b.txt", Size=25 }
            };

            var targetFiles = new List<FileDescriptor>
            {
                new FileDescriptor { FullPath="dest/a.txt", Size=10 },
                new FileDescriptor { FullPath="dest/b.txt", Size=20 }
            };

            var service = CreateService(sourceFiles, targetFiles);

            var toCopy = service.GetFilesToCopy("src", "dest");

            Assert.Equal(2, toCopy.Count);
            Assert.Contains(toCopy, f => f.FullPath == "src/a.txt");
            Assert.Contains(toCopy, f => f.FullPath == "src/b.txt");
        }
    }
}