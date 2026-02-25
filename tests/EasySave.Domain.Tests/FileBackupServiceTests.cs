using Xunit;
using System;
using System.IO;
using System.Collections.Generic;
using EasySave.Domain.Services;
using EasySave.Domain.Models;
using EasySave.Domain.Enums;
using EasySave.Domain.Exceptions;

public class FileBackupServiceTests : IDisposable
{
    private readonly string _testDirectory;
    private readonly FileBackupService _service;

    public FileBackupServiceTests()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_testDirectory);

        _service = new FileBackupService();
        _service.SetFilePath(_testDirectory);
    }

    // Cleanup after each test
    public void Dispose()
    {
        if (Directory.Exists(_testDirectory))
            Directory.Delete(_testDirectory, true);
    }

    // This test verifies that LoadJobs returns an empty list when the file does not exist.
    [Fact]
    public void LoadJobs_Should_Return_Empty_List_When_File_Not_Exists()
    {
        var jobs = _service.LoadJobs();

        Assert.NotNull(jobs);
        Assert.Empty(jobs);
    }

    // This test verifies that SaveJobs correctly creates and writes the jobs file.
    [Fact]
    public void SaveJobs_Should_Create_And_Write_File()
    {
        var jobs = new List<BackupJob>
        {
            new BackupJob(1, "Job1", @"C:\Source", @"D:\Target", BackupType.Full)
        };

        _service.SaveJobs(jobs);

        var filePath = Path.Combine(_testDirectory, "jobs.json");

        Assert.True(File.Exists(filePath));
        Assert.False(string.IsNullOrWhiteSpace(File.ReadAllText(filePath)));
    }

    // This test verifies that LoadJobs correctly deserializes saved jobs.
    [Fact]
    public void LoadJobs_Should_Return_Saved_Jobs()
    {
        var jobs = new List<BackupJob>
        {
            new BackupJob(1, "Job1", @"C:\Source", @"D:\Target", BackupType.Full)
        };

        _service.SaveJobs(jobs);

        var loadedJobs = _service.LoadJobs();

        Assert.Single(loadedJobs);
        Assert.Equal("Job1", loadedJobs[0].Name);
    }

    // This test verifies that LoadJobs returns an empty list when the file is empty.
    [Fact]
    public void LoadJobs_Should_Return_Empty_List_When_File_Is_Empty()
    {
        var filePath = Path.Combine(_testDirectory, "jobs.json");
        File.WriteAllText(filePath, "");

        var jobs = _service.LoadJobs();

        Assert.Empty(jobs);
    }

    // This test verifies that corrupted JSON throws a PersistenceException.
    [Fact]
    public void LoadJobs_Should_Throw_When_Json_Is_Corrupted()
    {
        var filePath = Path.Combine(_testDirectory, "jobs.json");
        File.WriteAllText(filePath, "{ invalid json }");

        Assert.Throws<PersistenceException>(() => _service.LoadJobs());
    }
}