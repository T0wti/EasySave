    using EasySave.Domain.Enums;
using System.Text.Json.Serialization;

namespace EasySave.Domain.Models
{
    // Represents a backup job configuration
    public class BackupJob
    {
        public int Id { get; init; }
        public string Name { get; init; }
        public string SourcePath { get; init; }
        public string TargetPath { get; init; }
        public BackupType Type { get; init; }

        // Constructor to initialize a backup job with required information
        [JsonConstructor]
        public BackupJob(int id, string name, string sourcePath, string targetPath, BackupType type)
        {
            Id = id;
            Name = name;
            SourcePath = sourcePath;
            TargetPath = targetPath;
            Type = type;
        }
    }
}
