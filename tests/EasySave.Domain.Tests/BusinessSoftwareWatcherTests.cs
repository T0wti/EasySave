using EasySave.Domain.Interfaces;
using EasySave.Domain.Services;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace EasySave.Tests.Domain.Services
{
    public class BusinessSoftwareWatcherTests
    {
        // Checks that if the software is already running, all handles are paused at startup
        [Fact]
        public async Task WatchAsync_Should_PauseAll_WhenSoftwareAlreadyRunning()
        {
            var softwareMock = new Mock<IBusinessSoftwareService>();
            var registryMock = new Mock<IBackupHandleRegistry>();
            var handleMock1 = new Mock<IBackupJobHandle>();
            var handleMock2 = new Mock<IBackupJobHandle>();

            softwareMock.Setup(s => s.GetConfiguredName()).Returns("Calculator");
            softwareMock.Setup(s => s.IsBusinessSoftwareRunning()).Returns(true);

            registryMock.Setup(r => r.GetAll())
                        .Returns(new[] {
                            (1, handleMock1.Object),
                            (2, handleMock2.Object)
                        });

            var watcher = new BusinessSoftwareWatcher(softwareMock.Object, registryMock.Object);
            using var cts = new CancellationTokenSource(200); // Stop quickly

            await watcher.WatchAsync(cts.Token);

            handleMock1.Verify(h => h.Pause(), Times.AtLeastOnce);
            handleMock2.Verify(h => h.Pause(), Times.AtLeastOnce);
        }

        // Checks that all handles are resumed when the watcher is canceled
        [Fact]
        public async Task WatchAsync_Should_ResumeAll_WhenCanceled()
        {
            var softwareMock = new Mock<IBusinessSoftwareService>();
            var registryMock = new Mock<IBackupHandleRegistry>();
            var handleMock1 = new Mock<IBackupJobHandle>();
            var handleMock2 = new Mock<IBackupJobHandle>();

            softwareMock.Setup(s => s.GetConfiguredName()).Returns("Calculator");
            softwareMock.Setup(s => s.IsBusinessSoftwareRunning()).Returns(false);

            registryMock.Setup(r => r.GetAll())
                        .Returns(new[] {
                            (1, handleMock1.Object),
                            (2, handleMock2.Object)
                        });

            var watcher = new BusinessSoftwareWatcher(softwareMock.Object, registryMock.Object);
            using var cts = new CancellationTokenSource(100);

            await watcher.WatchAsync(cts.Token);

            handleMock1.Verify(h => h.Resume(), Times.AtLeastOnce);
            handleMock2.Verify(h => h.Resume(), Times.AtLeastOnce);
        }

        // Checks that WatchAsync does not fail when no software name is configured
        [Fact]
        public async Task WatchAsync_ShouldNotFail_WhenNameIsEmpty()
        {
            var softwareMock = new Mock<IBusinessSoftwareService>();
            var registryMock = new Mock<IBackupHandleRegistry>();

            softwareMock.Setup(s => s.GetConfiguredName()).Returns(string.Empty);

            var watcher = new BusinessSoftwareWatcher(softwareMock.Object, registryMock.Object);
            using var cts = new CancellationTokenSource(50); // Cancel quickly

            // Act & Assert: catch the expected TaskCanceledException
            await Assert.ThrowsAsync<TaskCanceledException>(async () =>
            {
                await watcher.WatchAsync(cts.Token);
            });
        }
    }
}