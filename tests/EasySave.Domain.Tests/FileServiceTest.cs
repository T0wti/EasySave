using EasySave.Domain.Services;
using System;
using System.IO;
using Xunit;

namespace EasySave.Domain.Tests;
    
public class FileServiceTest
{
    [Fact]
    // GetFiles_ShouldReturnsAllFilesWithCorrectSize tests if the file path and the file size are retrieved correctly
    public void GetFiles_ShouldReturnsAllFilesWithCorrectSize()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempDir);

        try
        {
            var file1 = Path.Combine(tempDir, "file1.txt");
            var file2 = Path.Combine(tempDir, "file2.txt");

            File.WriteAllText(file1, "file1");
            File.WriteAllText(file2, "file2");

            var service = new FileService();
            var files = service.GetFiles(tempDir).ToList();

            var f1 = files.First(f => f.FullPath == file1);
            var f2 = files.First(f => f.FullPath == file2);

            Assert.Equal(2, files.Count);
            Assert.Equal(5, f1.Size);
            Assert.Equal(5, f2.Size);
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }


    [Fact]
    // CopyFile_ShouldCopyFileAndCreateDirectory copies the source file to the target and creates the directory if it does not exists
    public void CopyFile_ShouldCopyFileAndCreateDirectory()
    {
        var fileService = new FileService();

        var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        var sourceFile = Path.Combine(tempDir, "source.txt");
        var targetFile = Path.Combine(tempDir, "subdir", "target.txt");

        Directory.CreateDirectory(tempDir);
        File.WriteAllText(sourceFile, "test for CopyFile method without existing directory");

        fileService.CopyFile(sourceFile, targetFile);

        Assert.True(File.Exists(targetFile));
        var content = File.ReadAllText(targetFile);
        Assert.Equal("test for CopyFile method without existing directory", content);
    }

    [Fact]
    // CopyFile_ShouldOverwriteWhenExists tests if source file overwrite target when he exists
    public void CopyFile_ShouldOverwriteWhenTargetExists()
    {
        var fileService = new FileService();

        var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempDir);

        var sourceFile = Path.Combine(tempDir, "source.txt");
        var targetFile = Path.Combine(tempDir, "target.txt");

        File.WriteAllText(sourceFile, "test for CopyFile method : overwrite when target exists");
        File.WriteAllText(targetFile, "test for CopyFile method : need to be overwritten");



        fileService.CopyFile(sourceFile, targetFile);

        var content = File.ReadAllText(targetFile);
        Assert.Equal("test for CopyFile method : overwrite when target exists", content);
    }
}
