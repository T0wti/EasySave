using EasySave.Domain.Services;
using EasySave.Domain.Interfaces;
using EasySave.Domain.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace EasySave.Tests.Domain.Services
{
    public class PriorityGateTests
    {
        // IsPriority returns true if file extension matches configured priority extensions
        [Fact]
        public void IsPriority_ReturnsTrue_ForConfiguredExtension()
        {
            var configMock = new Mock<IConfigurationService>();
            configMock.Setup(c => c.LoadSettings()).Returns(new ApplicationSettings
            {
                PriorityFileExtensions = new List<string> { ".txt", ".doc" }
            });

            var gate = new PriorityGate(configMock.Object);

            bool result = gate.IsPriority("file.txt");
            Assert.True(result);

            result = gate.IsPriority("document.doc");
            Assert.True(result);
        }

        // IsPriority returns false for non-priority extensions
        [Fact]
        public void IsPriority_ReturnsFalse_ForNonConfiguredExtension()
        {
            var configMock = new Mock<IConfigurationService>();
            configMock.Setup(c => c.LoadSettings()).Returns(new ApplicationSettings
            {
                PriorityFileExtensions = new List<string> { ".txt", ".doc" }
            });

            var gate = new PriorityGate(configMock.Object);

            bool result = gate.IsPriority("image.png");
            Assert.False(result);
        }

        // WaitIfNeededAsync does not block priority files
        [Fact]
        public async Task WaitIfNeededAsync_PriorityFile_DoesNotBlock()
        {
            var configMock = new Mock<IConfigurationService>();
            configMock.Setup(c => c.LoadSettings()).Returns(new ApplicationSettings
            {
                PriorityFileExtensions = new List<string> { ".txt" }
            });

            var gate = new PriorityGate(configMock.Object);
            var cts = new CancellationTokenSource();

            await gate.WaitIfNeededAsync(true, cts.Token);
            // Success if no exception thrown
        }

        // Non-priority files block until priority files are done
        [Fact]
        public async Task WaitIfNeededAsync_NonPriorityFile_BlocksUntilPriorityFilesCopied()
        {
            var configMock = new Mock<IConfigurationService>();
            configMock.Setup(c => c.LoadSettings()).Returns(new ApplicationSettings
            {
                PriorityFileExtensions = new List<string> { ".txt" }
            });

            var gate = new PriorityGate(configMock.Object);

            // Register 1 priority file, closes the gate
            gate.RegisterPriorityFiles(1);

            bool reached = false;
            var cts = new CancellationTokenSource();
            var task = Task.Run(async () =>
            {
                await gate.WaitIfNeededAsync(false, cts.Token);
                reached = true;
            });

            await Task.Delay(100);
            Assert.False(reached);

            // Notify that priority file copied, gate opens
            gate.NotifyPriorityFileCopied();

            await task;
            Assert.True(reached); 
        }

        // NotifyPriorityFileCopied does not break when count is 0
        [Fact]
        public void NotifyPriorityFileCopied_DoesNothing_WhenNoPendingFiles()
        {
            var configMock = new Mock<IConfigurationService>();
            configMock.Setup(c => c.LoadSettings()).Returns(new ApplicationSettings());

            var gate = new PriorityGate(configMock.Object);

            Exception ex = Record.Exception(() => gate.NotifyPriorityFileCopied());
            Assert.Null(ex);
        }
    }
}