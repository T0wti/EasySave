using EasySave.Domain.Interfaces;
using EasySave.Domain.Models;
using EasySave.Domain.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace EasySave.Domain.Tests
{
    public class FullBackupStrategyTest
    {
        [Fact]
        // Should return all files from the source directory and call GetFiles once
        public void GetFilesToCopy_ShouldReturnAllSourceFiles()
        {
            var files = new List<FileDescriptor>
            {
                new FileDescriptor { FullPath="src/a.txt", Size=10 },
                new FileDescriptor { FullPath="src/b.txt", Size=20 }
            };

            var fileServiceMock = new Mock<IFileService>();
            fileServiceMock.Setup(f => f.GetFiles("src")).Returns(files);

            var strategy = new FullBackupStrategy(fileServiceMock.Object);

            var result = strategy.GetFilesToCopy("src", "dest");

            Assert.Equal(2, result.Count);
            Assert.Equal(files, result);

            fileServiceMock.Verify(f => f.GetFiles("src"), Times.Once);
        }

        [Fact]
        // Should return an empty list when the source directory contains no files
        public void GetFilesToCopy_WhenSourceIsEmpty()
        {
            var fileServiceMock = new Mock<IFileService>();
            fileServiceMock.Setup(f => f.GetFiles("src")).Returns(new List<FileDescriptor>());

            var strategy = new FullBackupStrategy(fileServiceMock.Object);

            var result = strategy.GetFilesToCopy("src", "dest");

            Assert.Empty(result);
        }

    }
}
