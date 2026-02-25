using EasySave.Domain.Enums;
using EasySave.Domain.Exceptions;
using EasySave.Domain.Interfaces;
using EasySave.Domain.Models;
using EasySave.Domain.Services;
using EasySave.EasyLog.Interfaces;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace EasySave.Tests
{
    public class BackupServiceTests
    {
        private readonly Mock<IFileService> _fileService = new();
        private readonly Mock<ILogService> _logService = new();
        private readonly Mock<IStateService> _stateService = new();
        private readonly Mock<IBackupStrategy> _fullStrategy = new();
        private readonly Mock<IBackupStrategy> _diffStrategy = new();
        private readonly Mock<IBusinessSoftwareWatcher> _watcher = new();
        private readonly Mock<ICryptoSoftService> _crypto = new();
        private readonly Mock<IPriorityGate> _priorityGate = new();
        private readonly Mock<ILargeSizeGate> _largeGate = new();

        private BackupService CreateService()
        {
            return new BackupService(
                _fileService.Object,
                _fullStrategy.Object,
                _diffStrategy.Object,
                _stateService.Object,
                _logService.Object,
                _watcher.Object,
                _crypto.Object,
                _priorityGate.Object,
                _largeGate.Object);
        }

        // Test that ExecuteBackup runs successfully for a full backup
        [Fact]
        public async Task ExecuteBackup_FullBackup_CompletesSuccessfully()
        {
            var service = CreateService();
            var job = new BackupJob(
                id: 1,
                name: "TestFull",
                sourcePath: "/source",
                targetPath: "/target",
                type: BackupType.Full
            );
            var handleMock = new Mock<IBackupJobHandle>();
            handleMock.Setup(h => h.CancellationToken).Returns(CancellationToken.None);

            var files = new List<FileDescriptor> { new FileDescriptor { FullPath = "/source/file1.txt", Size = 100 } };
            _fullStrategy.Setup(s => s.GetFilesToCopy(job.SourcePath, job.TargetPath)).Returns(files);

            await service.ExecuteBackup(job, handleMock.Object);

            _stateService.Verify(s => s.Complete(job.Id), Times.Once);
            _logService.Verify(l => l.Write(It.IsAny<LogEntry>()), Times.Once);
        }
    }
}