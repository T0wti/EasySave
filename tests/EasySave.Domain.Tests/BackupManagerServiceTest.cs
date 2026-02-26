using Xunit;
using Moq;
using System.Collections.Generic;
using System.Linq;
using EasySave.Domain.Services;
using EasySave.Domain.Interfaces;
using EasySave.Domain.Models;
using EasySave.Domain.Enums;
using EasySave.Domain.Exceptions;

public class BackupManagerServiceTests
{
    private readonly Mock<IFileBackupService> _fileBackupServiceMock;
    private readonly Mock<IBackupService> _backupServiceMock;
    private readonly ApplicationSettings _settings;

    private BackupManagerService _service;

    public BackupManagerServiceTests()
    {
        _fileBackupServiceMock = new Mock<IFileBackupService>();
        _backupServiceMock = new Mock<IBackupService>();
        _settings = new ApplicationSettings();

        _fileBackupServiceMock
            .Setup(x => x.LoadJobs())
            .Returns(new List<BackupJob>());

        _service = new BackupManagerService(
            _fileBackupServiceMock.Object,
            _backupServiceMock.Object,
            _settings
        );
    }

    // This test verifies that a valid backup job is successfully created and saved.
    [Fact]
    public void CreateBackupJob_Should_Create_Valid_Job()
    {
        _service.CreateBackupJob(
            "TestJob",
            @"C:\Source",
            @"D:\Target",
            BackupType.Full);

        var jobs = _service.GetBackupJobs();

        Assert.Single(jobs);
        Assert.Equal("TestJob", jobs.First().Name);

        _fileBackupServiceMock.Verify(x => x.SaveJobs(It.IsAny<List<BackupJob>>()), Times.Once);
    }

    // This test ensures that creating a job with an empty name throws a validation exception.
    [Fact]
    public void CreateBackupJob_Should_Throw_When_Name_Empty()
    {
        Assert.Throws<BackupValidationException>(() =>
            _service.CreateBackupJob(
                "",
                @"C:\Source",
                @"D:\Target",
                BackupType.Full));
    }

    // This test verifies that creating a job with an already existing name throws an exception.
    [Fact]
    public void CreateBackupJob_Should_Throw_When_Name_Already_Exists()
    {
        _service.CreateBackupJob(
            "TestJob",
            @"C:\Source",
            @"D:\Target",
            BackupType.Full);

        Assert.Throws<BackupJobAlreadyExistsException>(() =>
            _service.CreateBackupJob(
                "TestJob",
                @"C:\Source2",
                @"D:\Target2",
                BackupType.Full));
    }

    // This test ensures that a relative source path triggers a validation exception.
    [Fact]
    public void CreateBackupJob_Should_Throw_When_Source_Not_Absolute()
    {
        Assert.Throws<BackupValidationException>(() =>
            _service.CreateBackupJob(
                "Job",
                "relativePath",
                @"D:\Target",
                BackupType.Full));
    }

    // This test verifies that using the same source and target path throws a validation exception.
    [Fact]
    public void CreateBackupJob_Should_Throw_When_Source_Equals_Target()
    {
        Assert.Throws<BackupValidationException>(() =>
            _service.CreateBackupJob(
                "Job",
                @"C:\Same",
                @"C:\Same",
                BackupType.Full));
    }

    // This test verifies that an existing backup job is properly deleted and saved.
    [Fact]
    public void DeleteBackupJob_Should_Remove_Existing_Job()
    {
        _service.CreateBackupJob(
            "Job1",
            @"C:\Source",
            @"D:\Target",
            BackupType.Full);

        var jobId = _service.GetBackupJobs().First().Id;

        _service.DeleteBackupJob(jobId);

        Assert.Empty(_service.GetBackupJobs());
        _fileBackupServiceMock.Verify(x => x.SaveJobs(It.IsAny<List<BackupJob>>()), Times.Exactly(2));
    }

    // This test verifies that editing an existing job correctly updates its properties.
    [Fact]
    public void EditBackupJob_Should_Update_Existing_Job()
    {
        _service.CreateBackupJob(
            "Job1",
            @"C:\Source",
            @"D:\Target",
            BackupType.Full);

        var job = _service.GetBackupJobs().First();

        _service.EditBackupJob(
            job.Id,
            "UpdatedJob",
            @"C:\NewSource",
            @"D:\NewTarget",
            BackupType.Differential);

        var updated = _service.GetBackupJobs().First();

        Assert.Equal("UpdatedJob", updated.Name);
        Assert.Equal(@"C:\NewSource", updated.SourcePath);
        Assert.Equal(BackupType.Differential, updated.Type);

        _fileBackupServiceMock.Verify(x => x.SaveJobs(It.IsAny<List<BackupJob>>()), Times.Exactly(2));
    }

    // This test ensures that editing a non-existing job throws a not found exception.
    [Fact]
    public void EditBackupJob_Should_Throw_When_Not_Found()
    {
        Assert.Throws<BackupJobNotFoundException>(() =>
            _service.EditBackupJob(
                999,
                "Name",
                @"C:\Source",
                @"D:\Target",
                BackupType.Full));
    }

    // This test verifies that editing a job with a duplicate name throws an exception.
    [Fact]
    public void EditBackupJob_Should_Throw_When_Name_Already_Exists()
    {
        _service.CreateBackupJob(
            "Job1",
            @"C:\Source",
            @"D:\Target",
            BackupType.Full);

        _service.CreateBackupJob(
            "Job2",
            @"C:\Source2",
            @"D:\Target2",
            BackupType.Full);

        var job1 = _service.GetBackupJobs().First();

        Assert.Throws<BackupJobAlreadyExistsException>(() =>
            _service.EditBackupJob(
                job1.Id,
                "Job2",
                @"C:\Source",
                @"D:\Target",
                BackupType.Full));
    }
}