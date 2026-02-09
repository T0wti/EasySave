using EasySave.Domain.Enums;
using EasySave.Domain.Interfaces;
using EasySave.Domain.Models;
using EasySave.Domain.Services;
using EasySave.EasyLog.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace EasySave.Domain.Tests
{
    public class BackupServiceTest
    {
        [Fact]
        // Tests the backup job using the full backup strategy
        public void ExecuteBackup_FullStrategy()
        {
            var job = new BackupJob(1, "job1", "src", "dest", BackupType.Full);

            var files = new List<FileDescriptor>
                {
                    new FileDescriptor { FullPath = "src/a.txt", Size = 10 },
                    new FileDescriptor { FullPath = "src/b.txt", Size = 20 }
                };

            var fileServiceMock = new Mock<IFileService>();
            var fullStrategyMock = new Mock<IBackupStrategy>();
            var diffStrategyMock = new Mock<IBackupStrategy>();
            var fileBackupMock = new Mock<IFileBackupService>();
            var stateMock = new Mock<IStateService>();
            var logMock = new Mock<ILogService>();

            fullStrategyMock
                .Setup(s => s.GetFilesToCopy("src", "dest"))
                .Returns(files);

            fileBackupMock
                .Setup(f => f.LoadJobs())
                .Returns(new List<BackupJob> { job });

            var service = new BackupService(
                fileServiceMock.Object,
                fullStrategyMock.Object,
                diffStrategyMock.Object,
                fileBackupMock.Object,
                stateMock.Object,
                logMock.Object
            );

            service.ExecuteBackup(job);

            fullStrategyMock.Verify(s => s.GetFilesToCopy("src", "dest"), Times.Once);
            diffStrategyMock.Verify(s => s.GetFilesToCopy(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

            fileServiceMock.Verify(f => f.CopyFile(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
            logMock.Verify(l => l.WriteJson(It.IsAny<LogEntry>()), Times.Exactly(2));

            stateMock.Verify(s => s.Initialize(It.IsAny<BackupProgress>(), files), Times.Once);
            stateMock.Verify(s => s.Update(It.IsAny<BackupProgress>(), It.IsAny<FileDescriptor>(), It.IsAny<string>()), Times.Exactly(2));
            stateMock.Verify(s => s.Complete(job.Id), Times.Once);
        }


        [Fact]
        // Tests the backup job using the differential backup strategy
        public void ExecuteBackup_DifferentialStrategy()
        {
            var job = new BackupJob(1, "job1", "src", "dest", BackupType.Differential);

            var files = new List<FileDescriptor>
                {
                    new FileDescriptor { FullPath = "src/a.txt", Size = 10 },
                    new FileDescriptor { FullPath = "src/b.txt", Size = 20 }
                };

            var fileServiceMock = new Mock<IFileService>();
            var fullStrategyMock = new Mock<IBackupStrategy>();
            var diffStrategyMock = new Mock<IBackupStrategy>();
            var fileBackupMock = new Mock<IFileBackupService>();
            var stateMock = new Mock<IStateService>();
            var logMock = new Mock<ILogService>();

            diffStrategyMock.Setup(s => s.GetFilesToCopy("src", "dest"))
                    .Returns(files);

            fileBackupMock.Setup(f => f.LoadJobs())
                .Returns(new List<BackupJob> { job });

            var service = new BackupService(
                fileServiceMock.Object,
                fullStrategyMock.Object,
                diffStrategyMock.Object,
                fileBackupMock.Object,
                stateMock.Object,
                logMock.Object
            );

            service.ExecuteBackup(job);

            diffStrategyMock.Verify(s => s.GetFilesToCopy("src", "dest"), Times.Once);
            fullStrategyMock.Verify(s => s.GetFilesToCopy(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

            fileServiceMock.Verify(f => f.CopyFile(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
            logMock.Verify(l => l.WriteJson(It.IsAny<LogEntry>()), Times.Exactly(2));

            stateMock.Verify(s => s.Initialize(It.IsAny<BackupProgress>(), files), Times.Once);
            stateMock.Verify(s => s.Update(It.IsAny<BackupProgress>(), It.IsAny<FileDescriptor>(), It.IsAny<string>()), Times.Exactly(2));
            stateMock.Verify(s => s.Complete(job.Id), Times.Once);
        }

        [Fact]
        // Copy failure test, checks that the Fail method is called.
        public void ExecuteBackup_WhenCopyFails()
        {
            var job = new BackupJob(1, "job1", "src", "dest", BackupType.Full);

            var files = new List<FileDescriptor>
            { 
                new FileDescriptor { FullPath = "src/a.txt", Size = 10 }
            };

            var fileServiceMock = new Mock<IFileService>();
            var fullStrategyMock = new Mock<IBackupStrategy>();
            var diffStrategyMock = new Mock<IBackupStrategy>();
            var fileBackupMock = new Mock<IFileBackupService>();
            var stateMock = new Mock<IStateService>();
            var logMock = new Mock<ILogService>();

            fileServiceMock
                .Setup(f => f.CopyFile(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception("copy error"));

            fullStrategyMock.Setup(s => s.GetFilesToCopy("src", "dest")).Returns(files);

            fileBackupMock.Setup(f => f.LoadJobs())
                .Returns(new List<BackupJob>());

            var service = new BackupService(
                fileServiceMock.Object,
                fullStrategyMock.Object,
                diffStrategyMock.Object,
                fileBackupMock.Object,
                stateMock.Object,
                logMock.Object
            );

            Assert.Throws<Exception>(() => service.ExecuteBackup(job));

            stateMock.Verify(s => s.Fail(job.Id), Times.Once);
            logMock.Verify(l => l.WriteJson(It.IsAny<LogEntry>()), Times.Once);
        }

        [Fact]
        // Tests that ExecuteBackups call ExecuteBackup
        public void ExecuteBackups_ShouldExecuteAllJobs()
        {
            var job1 = new BackupJob(1, "job1", "src", "dest", BackupType.Full);
            var job2 = new BackupJob(2, "job2", "src", "dest", BackupType.Full);

            var jobs = new List<BackupJob> { job1, job2 };

            var files = new List<FileDescriptor>
            {
                new FileDescriptor { FullPath = "src/a.txt", Size = 10 }
            };

            var fileServiceMock = new Mock<IFileService>();
            var fullStrategyMock = new Mock<IBackupStrategy>();
            var diffStrategyMock = new Mock<IBackupStrategy>();
            var fileBackupMock = new Mock<IFileBackupService>();
            var stateMock = new Mock<IStateService>();
            var logMock = new Mock<ILogService>();

            fullStrategyMock
                .Setup(s => s.GetFilesToCopy("src", "dest"))
                .Returns(files);

            fileBackupMock
                .Setup(f => f.LoadJobs())
                .Returns(jobs);

            var service = new BackupService(
                fileServiceMock.Object,
                fullStrategyMock.Object,
                diffStrategyMock.Object,
                fileBackupMock.Object,
                stateMock.Object,
                logMock.Object
            );

            service.ExecuteBackups(jobs);

            fileServiceMock.Verify(
                f => f.CopyFile(It.IsAny<string>(), It.IsAny<string>()),
                Times.Exactly(2)
            );

            stateMock.Verify(
                s => s.Complete(It.IsAny<int>()),
                Times.Exactly(2)
            );

        }
    }
}
