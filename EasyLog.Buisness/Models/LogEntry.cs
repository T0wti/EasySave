namespace EasyLog.Buisness;

// LogEntry represents the data model that who will form the JSON structure.
public class LogEntry
{
    public DateTime Timestamp { get; set; }
    public string BackupName { get; set; }
    public string SourcePath { get; set; } // UNC
    public string DestinationPath { get; set; } // UNC
    public long FileSize { get; set; }
    public int TransfertTime { get; set; } // MS + négatif si erreur
}
