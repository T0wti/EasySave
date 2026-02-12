using EasySave.Domain.Enums;
using System.Text.Json.Serialization;

namespace EasySave.Domain.Models
{
    // Represents the runtime progress of a backup job
    public class BackupProgress
    {
        public string BackupName { get; set; }
        public int BackupJobId { get; init; }
        public BackupJobState State { get; set; }
        public DateTime LastUpdate { get; set; }


        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int TotalFiles { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public long TotalSize { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int RemainingFiles { get;  set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public long RemainingSize { get;  set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public double Progression { get;  set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? CurrentSourceFile { get;  set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? CurrentTargetFile { get;  set; }

        public BackupProgress() { }

        // Factory method to create a BackupProgress instance from a BackupJob
        public static BackupProgress From(BackupJob job)
        {
            return new BackupProgress
            {
                BackupJobId = job.Id,
                BackupName = job.Name
            };
        }
    }
}

