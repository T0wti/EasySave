using EasySave.Domain.Interfaces;
using EasySave.Domain.Services;
using Moq;
using System.Linq;
using Xunit;

namespace EasySave.Tests.Domain.Services
{
    public class BackupHandleRegistryTests
    {
        // Checks that Register adds a handle and Get returns it
        [Fact]
        public void Register_Should_AddHandle_And_Get_ReturnsIt()
        {
            var registry = new BackupHandleRegistry();
            var mockHandle = new Mock<IBackupJobHandle>();

            registry.Register(1, mockHandle.Object);

            var retrieved = registry.Get(1);

            Assert.NotNull(retrieved);
            Assert.Equal(mockHandle.Object, retrieved);
        }

        // Checks that Get returns null for non-existent job
        [Fact]
        public void Get_ShouldReturnNull_WhenHandleDoesNotExist()
        {
            var registry = new BackupHandleRegistry();

            var retrieved = registry.Get(999);

            Assert.Null(retrieved);
        }

        // Checks that Unregister removes the handle and disposes it
        [Fact]
        public void Unregister_Should_RemoveHandle_AndDisposeIt()
        {
            var registry = new BackupHandleRegistry();
            var mockHandle = new Mock<IBackupJobHandle>();

            registry.Register(1, mockHandle.Object);
            registry.Unregister(1);

            var retrieved = registry.Get(1);

            Assert.Null(retrieved);
            mockHandle.Verify(h => h.Dispose(), Times.Once);
        }

        // Checks that GetAll returns all active handles
        [Fact]
        public void GetAll_Should_ReturnAllActiveHandles()
        {
            var registry = new BackupHandleRegistry();
            var mockHandle1 = new Mock<IBackupJobHandle>();
            var mockHandle2 = new Mock<IBackupJobHandle>();

            registry.Register(1, mockHandle1.Object);
            registry.Register(2, mockHandle2.Object);

            var all = registry.GetAll().ToList();

            Assert.Equal(2, all.Count);
            Assert.Contains(all, h => h.JobId == 1 && h.Handle == mockHandle1.Object);
            Assert.Contains(all, h => h.JobId == 2 && h.Handle == mockHandle2.Object);
        }

        // Checks that Unregister on a non-existent job does not throw
        [Fact]
        public void Unregister_ShouldNotThrow_WhenHandleDoesNotExist()
        {
            var registry = new BackupHandleRegistry();

            var ex = Record.Exception(() => registry.Unregister(999));

            Assert.Null(ex);
        }
    }
}