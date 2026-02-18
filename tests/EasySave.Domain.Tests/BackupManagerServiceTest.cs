using EasySave.Domain.Enums;
using EasySave.Domain.Interfaces;
using EasySave.Domain.Models;
using EasySave.Domain.Services;
using Moq;
using Xunit;
using System.Collections.Generic;
using System.IO;

public class BackupManagerServiceTests
{
    // This method instantiates a BackupManagerService with mocked services
    private BackupManagerService CreateService(
        List<BackupJob> existingJobs)
    {
        var fileMock = new Mock<IFileBackupService>();
        fileMock.Setup(f => f.LoadJobs()).Returns(existingJobs);

        var backupMock = new Mock<IBackupService>();

        var settings = new ApplicationSettings();

        return new BackupManagerService(
            fileMock.Object,
            backupMock.Object,
            settings
        );
    }

    [Fact]
    // Raises the name exception
    public void CreateBackupJob_WhenNameExists()
    {
        var jobs = new List<BackupJob>
        {
            new BackupJob(1, "job1", "src", "dest", BackupType.Differential)
        };

        var service = CreateService(jobs);

        Assert.Throws<Exception>(() =>
            service.CreateBackupJob("job1", "srctest", "srcdest", BackupType.Differential));
    }

    [Fact]
    // Tests if SaveJobs method is called
    public void CreateBackupJob_ShouldAddJob()
    {
        var jobs = new List<BackupJob>();
        var fileMock = new Mock<IFileBackupService>();
        fileMock.Setup(f => f.LoadJobs()).Returns(jobs);

        var backupMock = new Mock<IBackupService>();

        var settings = new ApplicationSettings();

        var service = new BackupManagerService(
            fileMock.Object,
            backupMock.Object,
            settings
        );

        service.CreateBackupJob("job1", "src", "dest", BackupType.Full);

        Assert.Single(service.GetBackupJobs());
        fileMock.Verify(
            f => f.SaveJobs(It.IsAny<List<BackupJob>>()), 
            Times.Once
        );
    }

    [Fact]
    // Checks if the selected backup task has been deleted.
    public void DeleteBackupJob_WhenBackupJobExists()
    {
        var jobs = new List<BackupJob>
        {
            new BackupJob(1,"job1","src","dest",BackupType.Full),
            new BackupJob(2,"job2","src","dest",BackupType.Full),
        };

        var service = CreateService(jobs);
        service.DeleteBackupJob(1);
        Assert.True(jobs.Count == 1);
    }

    [Fact]
    // Checks that the backup job is deleted and that SaveJobs methods is called
    public void DeleteBackupJob_WhenBackupJobDoesNotExists()
    {
        var jobs = new List<BackupJob>
        {
            new BackupJob(1,"job1","src","dest",BackupType.Full),
        };

        var fileMock = new Mock<IFileBackupService>();
        fileMock.Setup(f => f.LoadJobs()).Returns(jobs);

        var service = new BackupManagerService(
            fileMock.Object,
            new Mock<IBackupService>().Object,
            new ApplicationSettings()
        );

        service.DeleteBackupJob(2);

        Assert.Single(service.GetBackupJobs());
        fileMock.Verify(
            f => f.SaveJobs(It.IsAny<List<BackupJob>>()),
            Times.Never
        );
    }

    [Fact]
    // Checks that the backup job is edited and that SaveJobs methods is called 
    public void EditBackupJob_WhenValid()
    {
        var jobs = new List<BackupJob>
        {
            new BackupJob(1,"job1","src","dest",BackupType.Full)
        };

        var fileMock = new Mock<IFileBackupService>();
        fileMock.Setup(f => f.LoadJobs()).Returns(jobs);

        var service = new BackupManagerService(
            fileMock.Object,
            new Mock<IBackupService>().Object,
            new ApplicationSettings()
        );

        service.EditBackupJob(1, "job1-edited", "src-edited", "dest-edited", BackupType.Full);

        var editedJob = service.GetBackupJobs()[0];
        Assert.Equal("job1-edited", editedJob.Name);
        Assert.Equal("src-edited", editedJob.SourcePath);
        Assert.Equal("dest-edited", editedJob.TargetPath);
        Assert.Equal(BackupType.Full, editedJob.Type);

        fileMock.Verify(f => f.SaveJobs(It.IsAny<List<BackupJob>>()), Times.Once);
    }

    [Fact]
    // Raises the id job exception 
    public void EditBackupJob_WhenJobDoesNotExists()
    {
        var jobs = new List<BackupJob>
        {
             new BackupJob(1,"job1","src","dest",BackupType.Differential)
        };

        var service = CreateService(jobs);

        Assert.Throws<Exception>(() =>
            service.EditBackupJob(2, "job2", "src", "dest", BackupType.Full)
        );
    }

    [Fact]
    // Raises the name job exception 
    public void EditBackupJob_WhenNameAlreadyExists()
    {
        var jobs = new List<BackupJob>
        {
            new BackupJob(1,"job1","src","dest",BackupType.Full),
            new BackupJob(2,"job2","src","dest",BackupType.Full)
        };

        var service = CreateService(jobs);

        Assert.Throws<Exception>(() =>
            service.EditBackupJob(1, "job2", "src", "dest", BackupType.Full)
        );
    }

    [Fact]
    // Checks that ExecuteBackup method is called
    public void ExecuteBackupJob_WhenJobExists()
    {
        var jobs = new List<BackupJob>
        {
            new BackupJob(1,"job1","src","dest",BackupType.Full)
        };

        var fileMock = new Mock<IFileBackupService>();
        fileMock.Setup(f => f.LoadJobs()).Returns(jobs);

        var backupMock = new Mock<IBackupService>();

        var service = new BackupManagerService(
            fileMock.Object, 
            backupMock.Object,
            new ApplicationSettings()
        );

        service.ExecuteBackupJob(1);

        backupMock.Verify(b => b.ExecuteBackup(It.Is<BackupJob>(j => j.Id == 1)), Times.Once);
    }

    [Fact]
    // Raises the id job exception 
    public void ExecuteBackupJob_WhenJobDoesNotExist()
    {
        var jobs = new List<BackupJob>
        {
            new BackupJob(1,"job1","src","dest",BackupType.Full)
        };

        var service = CreateService(jobs);

        Assert.Throws<Exception>(() => service.ExecuteBackupJob(2));
    }

    [Fact]
    // Checks that the ExecuteBackupJob method is called for each BackupJob
    public void ExecuteBackupJobs_WhenJobsDoesNotExists()
    {
        var jobs = new List<BackupJob>
        {
            new BackupJob(1,"job1","src","dest",BackupType.Full),
            new BackupJob(2,"job2","src","dest",BackupType.Full)
        };

        var fileMock = new Mock<IFileBackupService>();
        fileMock.Setup(f => f.LoadJobs()).Returns(jobs);

        var backupMock = new Mock<IBackupService>();

        var service = new BackupManagerService(
            fileMock.Object,
            backupMock.Object,
            new ApplicationSettings()
        );

        service.ExecuteBackupJobs(jobs);

        backupMock.Verify(b => b.ExecuteBackup(It.Is<BackupJob>(j => j.Id == 1)), Times.Once);
        backupMock.Verify(b => b.ExecuteBackup(It.Is<BackupJob>(j => j.Id == 2)), Times.Once);
    }
}