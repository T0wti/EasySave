using EasySave.Domain.Models;
using EasySave.Domain.Services;
using EasySave.Domain.Interfaces;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace EasySave.Tests.Domain.Services
{
    public class LargeSizeGateTests
    {
        // IsLargeFile returns true for files larger than configured threshold
        [Fact]
        public void IsLargeFile_ReturnsTrue_WhenFileExceedsThreshold()
        {
            var configMock = new Mock<IConfigurationService>();
            configMock.Setup(c => c.LoadSettings()).Returns(new ApplicationSettings
            {
                MaxLargeFileSizeKb = 1 // 1 KB threshold
            });

            var gate = new LargeSizeGate(configMock.Object);

            bool result = gate.IsLargeFile(2 * 1024); // 2 KB
            Assert.True(result);
        }

        // IsLargeFile returns false for files smaller than threshold
        [Fact]
        public void IsLargeFile_ReturnsFalse_WhenFileBelowThreshold()
        {
            var configMock = new Mock<IConfigurationService>();
            configMock.Setup(c => c.LoadSettings()).Returns(new ApplicationSettings
            {
                MaxLargeFileSizeKb = 10 // 10 KB threshold
            });

            var gate = new LargeSizeGate(configMock.Object);

            bool result = gate.IsLargeFile(5 * 1024); // 5 KB
            Assert.False(result);
        }

        // AcquireIfLargeAsync blocks for large files but not for small files
        [Fact]
        public async Task AcquireIfLargeAsync_BlocksOnlyForLargeFiles()
        {
            var configMock = new Mock<IConfigurationService>();
            configMock.Setup(c => c.LoadSettings()).Returns(new ApplicationSettings
            {
                MaxLargeFileSizeKb = 1 // 1 KB threshold
            });

            var gate = new LargeSizeGate(configMock.Object);

            // Small file should not block
            var cts = new CancellationTokenSource();
            await gate.AcquireIfLargeAsync(512, cts.Token); // 512 bytes < 1 KB
            gate.ReleaseIfLarge(512);

            // Large file should acquire semaphore
            await gate.AcquireIfLargeAsync(2 * 1024, cts.Token); // 2 KB > 1 KB

            bool blocked = false;
            var task = Task.Run(async () =>
            {
                blocked = true;
                await gate.AcquireIfLargeAsync(2 * 1024, cts.Token);
                blocked = false;
            });

            await Task.Delay(100);
            Assert.True(blocked, "Second large file should be blocked");

            gate.ReleaseIfLarge(2 * 1024);
            await task;
            Assert.False(blocked, "After release, second large file should proceed");
        }

        // ReleaseIfLarge does not throw for small files
        [Fact]
        public void ReleaseIfLarge_DoesNothing_ForSmallFile()
        {
            var configMock = new Mock<IConfigurationService>();
            configMock.Setup(c => c.LoadSettings()).Returns(new ApplicationSettings
            {
                MaxLargeFileSizeKb = 1 // 1 KB threshold
            });

            var gate = new LargeSizeGate(configMock.Object);

            Exception ex = Record.Exception(() => gate.ReleaseIfLarge(512)); // 512 bytes < 1 KB
            Assert.Null(ex);
        }
    }
}