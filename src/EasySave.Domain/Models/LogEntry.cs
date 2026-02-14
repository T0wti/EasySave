using System;
using System.Collections.Generic;
using System.Text;

namespace EasySave.Domain.Models
{
    // Represents a single log entry for a file backup operation
    public class LogEntry
    {
        public DateTime Timestamp { get; init; }
        public string BackupName { get; init; }
        public string SourcePath { get; init; }
        public string TargetPath { get; init; }
        public long FileSize { get; init; }
        public long TransferTimeMs { get; init; }
        public long EncryptionTimeMs { get; init; }
    }

}
