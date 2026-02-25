using EasySave.Domain.Services;
using EasySave.Domain.Interfaces;
using EasySave.Domain.Models;
using EasySave.Domain.Enums;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace EasySave.Tests.Domain.Services
{
    public class StateServiceTests
    {
        private readonly Mock<IFileStateService> _fileStateMock;
        private readonly StateService _service;

        public StateServiceTests()
        {
            _fileStateMock = new Mock<IFileStateService>();
            _fileStateMock.Setup(f => f.ReadState()).Returns(new List<BackupProgress>());

            _service = new StateService(_fileStateMock.Object);
        }

        // Initialize sets all runtime fields correctly
        [Fact]
        public void Initialize_SetsBackupProgressCorrectly()
        {
            var progress = new BackupProgress { BackupJobId = 1, State = BackupJobState.Inactive };
            var files = new List<FileDescriptor>
            {
                new FileDescriptor { FullPath = "a.txt", Size = 100 },
                new FileDescriptor { FullPath = "b.txt", Size = 200 }
            };

            _service.Initialize(progress, files);

            Assert.Equal(BackupJobState.Active, progress.State);
            Assert.Equal(2, progress.TotalFiles);
            Assert.Equal(300, progress.TotalSize);
            Assert.Equal(2, progress.RemainingFiles);
            Assert.Equal(300, progress.RemainingSize);
            Assert.Equal(0, progress.Progression);

            _fileStateMock.Verify(f => f.WriteState(It.Is<List<BackupProgress>>(l => l.Contains(progress))), Times.Once);
        }

        // Update updates remaining files, size, progression, and current file
        [Fact]
        public void Update_UpdatesProgressCorrectly()
        {
            var progress = new BackupProgress { BackupJobId = 1, TotalFiles = 2, TotalSize = 300, RemainingFiles = 2, RemainingSize = 300 };
            var file = new FileDescriptor { FullPath = "a.txt", Size = 100 };

            _service.Update(progress, file, @"D:\target\a.txt");

            Assert.Equal(1, progress.RemainingFiles);
            Assert.Equal(200, progress.RemainingSize);
            Assert.Equal(33.33, progress.Progression, 2);
            Assert.Equal("a.txt", progress.CurrentSourceFile);
            Assert.Equal(@"D:\target\a.txt", progress.CurrentTargetFile);

            _fileStateMock.Verify(f => f.WriteState(It.Is<List<BackupProgress>>(l => l.Contains(progress))), Times.Once);
        }

        // Pause updates the state to Paused
        [Fact]
        public void Pause_UpdatesStateToPaused()
        {
            var progress = new BackupProgress { BackupJobId = 1, State = BackupJobState.Active };
            _fileStateMock.Setup(f => f.ReadState()).Returns(new List<BackupProgress> { progress });

            _service.Pause(1);

            Assert.Equal(BackupJobState.Paused, progress.State);
            _fileStateMock.Verify(f => f.WriteState(It.IsAny<List<BackupProgress>>()), Times.Once);
        }

        // Stop updates the state to Stopped
        [Fact]
        public void Stop_UpdatesStateToStopped()
        {
            var progress = new BackupProgress { BackupJobId = 1, State = BackupJobState.Active };
            _fileStateMock.Setup(f => f.ReadState()).Returns(new List<BackupProgress> { progress });

            _service.Stop(1);

            Assert.Equal(BackupJobState.Stopped, progress.State);
            _fileStateMock.Verify(f => f.WriteState(It.IsAny<List<BackupProgress>>()), Times.Once);
        }

        // Complete sets state to Completed and clears runtime fields
        [Fact]
        public void Complete_FinalizesAndCleansProgress()
        {
            var progress = new BackupProgress { BackupJobId = 1, State = BackupJobState.Active, TotalFiles = 2, TotalSize = 200, RemainingFiles = 1, RemainingSize = 100, CurrentSourceFile = "a", CurrentTargetFile = "b" };
            _fileStateMock.Setup(f => f.ReadState()).Returns(new List<BackupProgress> { progress });

            _service.Complete(1);

            Assert.Equal(BackupJobState.Completed, progress.State);
            Assert.Equal(0, progress.TotalFiles);
            Assert.Equal(0, progress.TotalSize);
            Assert.Null(progress.CurrentSourceFile);
            Assert.Null(progress.CurrentTargetFile);

            _fileStateMock.Verify(f => f.WriteState(It.IsAny<List<BackupProgress>>()), Times.Once);
        }

        // Fail sets state to Failed
        [Fact]
        public void Fail_UpdatesStateToFailed()
        {
            var progress = new BackupProgress { BackupJobId = 1, State = BackupJobState.Active };
            _fileStateMock.Setup(f => f.ReadState()).Returns(new List<BackupProgress> { progress });

            _service.Fail(1);

            Assert.Equal(BackupJobState.Failed, progress.State);
        }

        // Interrupt sets state to Interrupted and clears runtime fields
        [Fact]
        public void Interrupt_FinalizesAndCleansProgress()
        {
            var progress = new BackupProgress { BackupJobId = 1, State = BackupJobState.Active, TotalFiles = 5 };
            _fileStateMock.Setup(f => f.ReadState()).Returns(new List<BackupProgress> { progress });

            _service.Interrupt(1);

            Assert.Equal(BackupJobState.Interrupted, progress.State);
            Assert.Equal(0, progress.TotalFiles);
        }

        // Compare sets state to Comparing
        [Fact]
        public void Compare_UpdatesStateToComparing()
        {
            var progress = new BackupProgress { BackupJobId = 1, State = BackupJobState.Active };
            _fileStateMock.Setup(f => f.ReadState()).Returns(new List<BackupProgress> { progress });

            _service.Compare(1);

            Assert.Equal(BackupJobState.Comparing, progress.State);
        }
    }
}