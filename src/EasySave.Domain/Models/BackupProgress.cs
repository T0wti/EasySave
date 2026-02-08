using EasySave.Domain.Enums;
using System.Text.Json.Serialization;

namespace EasySave.Domain.Models
{
    public class BackupProgress
    {
        public string BackupName { get; set; }
        public int BackupJobId { get; init; }
        public BackupJobState State { get; set; }
        public DateTime LastUpdate { get; internal set; }


        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int TotalFiles { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public long TotalSize { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int RemainingFiles { get; internal set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public long RemainingSize { get; internal set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public double Progression { get; internal set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? CurrentSourceFile { get; internal set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? CurrentTargetFile { get; internal set; }

        public BackupProgress() { }

        public static BackupProgress From(BackupJob job)
        {
            return new BackupProgress
            {
                BackupJobId = job.Id,
                BackupName = job.Name,
                State = BackupJobState.Active,
                LastUpdate = DateTime.Now,
                Progression = 0
            };
        }
    }
}

