using EasySave.Domain.Enums;

namespace EasySave.Domain.Models
{
    public class BackupProgress
    {
        public string BackupName { get; set; }
        public int BackupJobId { get; init; }
        public BackupJobState State { get; set; }
        public DateTime LastUpdate { get; internal set; }


        public int TotalFiles { get; internal set; }
        public long TotalSize { get; internal set; }

        public int RemainingFiles { get; internal set; }
        public long RemainingSize { get; internal set; }

        public string? CurrentSourceFile { get; internal set; }
        public string? CurrentTargetFile { get; internal set; }

        public BackupProgress() { }

        public static BackupProgress From(BackupJob job)
        {
            return new BackupProgress
            {
                BackupJobId = job.Id,
                BackupName = job.Name,
                State = BackupJobState.Active,
                LastUpdate = DateTime.Now
            };
        }
    }
}

