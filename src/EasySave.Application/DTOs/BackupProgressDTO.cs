namespace EasySave.Application.DTOs
{
    public class BackupProgressDTO
    {
        public int BackupJobId { get; set; }
        public string BackupName { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public double Progression { get; set; }
        public int TotalFiles { get; set; }
        public int RemainingFiles { get; set; }
        public long TotalSize { get; set; }
        public long RemainingSize { get; set; }
        public string? CurrentSourceFile { get; set; }
        public string? CurrentTargetFile { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}