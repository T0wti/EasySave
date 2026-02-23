using EasySave.Domain.Enums;

namespace EasySave.Domain.Models
{
    public class ApplicationSettings
    {
        public Language Language { get; set; }
        public string LogDirectoryPath { get; set; } = string.Empty;
        public string StateFileDirectoryPath { get; set; } = string.Empty;
        public int LogFormat { get; set; }    
        public int LogMode { get; set; }
        public string? LogServerHost { get; set; }
        public int LogServerPort { get; set; }
        public string? BusinessSoftwareName { get; set; }
        public List<string>? EncryptedFileExtensions { get; set; }
        public List<string>? PriorityFileExtensions { get; set; }
        public long MaxLargeFileSizeKb { get; set; } = 0;
        public string? CryptoSoftPath { get; set; }
        public string? CryptoSoftKeyPath { get; set; }
    }
}